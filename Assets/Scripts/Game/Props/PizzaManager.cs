using UnityEngine;
using UnityEngine.Rendering;

public class PizzaManager : MonoBehaviour
    {
        public static PizzaManager Instance { get; private set; }
        private GameObject playerCharacter;
        
        public int totalPizzas;
        private int remainingPizzas;
        public GameObject gameOverPanel;
        public UIManager uiManager;
        
        void Start()
            {
                remainingPizzas = totalPizzas;
                Instance = this;
            }
        
        public void PizzaDestroyed(Pizza pizza)
            {
                print("Pizza Destroyed");
                remainingPizzas -= 1;
                CheckCurrentPizzaCount();
                uiManager.RemoveLastPizzaIndicator();
            }

        private void CheckCurrentPizzaCount()
            {
                if (remainingPizzas <= 0)
                    {
                        print("Game Over");
                        ShowGameOver();
                    }
            }

        private void ShowGameOver()
            {
                Time.timeScale = 0;
                
                playerCharacter = GameObject.FindWithTag("Player");
                playerCharacter.SetActive(false);
                SceneTransitionManager.Instance.DissolveToScene("GameOverScene");
            }
    }