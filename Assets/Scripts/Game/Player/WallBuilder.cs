using UnityEngine;
using System.Collections.Generic;
using System.Linq; // List 변환을 위해 추가
using UnityEngine.UI;

public class WallBuilder : MonoBehaviour
{
	public static WallBuilder Instance;

	ActionStateManager state;

	public GameObject wallPrefab;
	public GameObject wallPreviewPrefab;
	public LayerMask buildableLayer;
	public LayerMask wallLayer;
	public LayerMask pizzaLayer;

	public int maxWalls = 3;

	public float cooldownTime = 10f; // 스킬 쿨타임 시간
	private float cooldownTimer;
	private bool isCooldown;


	private GameObject currentPreview;
	public bool isBuildingEnabled = false;
	private Queue<GameObject> builtWalls = new Queue<GameObject>();
	private Vector3 lastValidPosition;

	bool canBuild;

	private void Awake()
	{
		Instance = this;
		state = GetComponent<ActionStateManager>();	
	}

	void Start()
	{
		if (LayerMask.NameToLayer("Wall") == -1)
		{
			Debug.LogError("Wall layer does not exist. Please create it in Unity's Layer settings.");
		}

		WallHP.OnWallDestroyed += HandleWallDestroyed; // 이벤트 구독
	}

	void OnDestroy()
	{
		WallHP.OnWallDestroyed -= HandleWallDestroyed; // 이벤트 구독 해제
	}

	void Update()
	{
		if (isCooldown)
        {
			isBuildingEnabled = false;
			cooldownTimer -= Time.deltaTime;
			UIManager.Instance.coolDownText.text = ((cooldownTimer / cooldownTime) * 10).ToString("F1");
            if (cooldownTimer <= 0)
            {
				UIManager.Instance.coolDownText.text = "";
				isCooldown = false;
                UIManager.Instance.coolDownImage.fillAmount = 0;
            }
            else
            {
				UIManager.Instance.coolDownImage.fillAmount = cooldownTimer / cooldownTime;
            }
        }

		if (Input.GetKeyDown(KeyCode.Q) && state.currentState != state.Reload)
		{
			state.SetLayerWeight(0, 1);
			ToggleBuildMode();
		}

		if (isBuildingEnabled && state.currentState != state.Reload)
		{
			UpdatePreview();

			if (canBuild && Input.GetMouseButtonDown(0) && !isCooldown && state.currentState != state.Reload)
			{
				BuildWall();
			}
		}
		else if (currentPreview != null)
		{
			Destroy(currentPreview);
			currentPreview = null;
		}
	}

	void ToggleBuildMode()
	{
		if (isCooldown) return;
		isBuildingEnabled = !isBuildingEnabled;
		Debug.Log("Building mode: " + (isBuildingEnabled ? "On" : "Off"));
	}

	void UpdatePreview()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100f, buildableLayer))
		{
			Vector3 buildPosition = hit.point;
			buildPosition.y = Mathf.Round(buildPosition.y);

			float wallHeight = wallPrefab.GetComponent<Renderer>().bounds.size.y;
			buildPosition.y += wallHeight / 2 - 2.5f;

			if (currentPreview == null)
			{
				currentPreview = Instantiate(wallPreviewPrefab, buildPosition, Quaternion.identity);
				currentPreview.layer = LayerMask.NameToLayer("PreviewWall");
			}

			canBuild = !IsOverlapping(buildPosition);

			if (canBuild)
			{
				lastValidPosition = buildPosition;
				currentPreview.transform.position = buildPosition;
			}
			else
			{
				currentPreview.transform.position = lastValidPosition;
			}

			Renderer previewRenderer = currentPreview.GetComponent<Renderer>();
			if (previewRenderer != null)
			{
				previewRenderer.material.color =
					canBuild ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
			}
		}
	}

	void BuildWall()
	{
		if (currentPreview != null && !IsOverlapping(currentPreview.transform.position))
		{
			if(this.gameObject.name == "Kafka(Clone)")
			{
				PlayerSoundSource.Instance.GetUseKafkaSkillSound();
			}
			else if (this.gameObject.name == "Serval(Clone)")
			{
				PlayerSoundSource.Instance.GetUseServalSkillSound();
			}
			else if (this.gameObject.name == "Yanqing(Clone)")
				PlayerSoundSource.Instance.GetUseYanqingSkillSound();

			GameObject newWall = Instantiate(wallPrefab,
				currentPreview.transform.position + new Vector3(0, 5, 0), Quaternion.identity);
			newWall.layer = LayerMask.NameToLayer("Wall");

			if (builtWalls.Count >= maxWalls)
			{
				DestroyBlock();
			}

			builtWalls.Enqueue(newWall);

			isCooldown = true;
			cooldownTimer = cooldownTime;
			UIManager.Instance.coolDownImage.fillAmount = 1;
		}
	}

	public void UseRPC_AddBlock()
	{
		GameObject newWall = Instantiate(wallPrefab, currentPreview.transform.position,
			Quaternion.identity);
		newWall.layer = LayerMask.NameToLayer("Wall");
		newWall.SetActive(false);
		builtWalls.Enqueue(newWall);
	}

	public void DestroyBlock()
	{
		GameObject oldestWall = builtWalls.Dequeue();
		Destroy(oldestWall);
	}

	bool IsOverlapping(Vector3 position)
	{
		Collider[] wallColliders = Physics.OverlapBox(position,
			wallPrefab.GetComponent<Renderer>().bounds.extents * 0.9f, Quaternion.identity, wallLayer);
		Collider[] pizzaColliders = Physics.OverlapBox(position,
			wallPrefab.GetComponent<Renderer>().bounds.extents * 0.9f, Quaternion.identity, pizzaLayer);
		return wallColliders.Length > 0 || pizzaColliders.Length > 0;
	}

	private void HandleWallDestroyed(GameObject wall)
	{
		if (builtWalls.Contains(wall))
		{
			// Queue를 List로 변환
			List<GameObject> tempList = builtWalls.ToList();
			// List에서 제거
			tempList.Remove(wall);
			// 다시 Queue로 변환
			builtWalls = new Queue<GameObject>(tempList);
			Debug.Log("Wall destroyed and removed from queue.");
		}

		Destroy(wall);
	}
}