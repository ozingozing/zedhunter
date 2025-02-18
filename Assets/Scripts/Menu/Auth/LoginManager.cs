using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
    {
        private string loginEndpoint = "http://52.78.204.71:13756/account/login";
        private string createEndpoint = "http://52.78.204.71:13756/account/create";

        public static string LoginEndpoint { get; private set; }
        
        public MenuManager menuManager;
        private ImageUploader _imageUploader;
        
        [Header("Creating Account")]
        [SerializeField] private TextMeshProUGUI loginAlertText;
        [SerializeField] private TextMeshProUGUI createAlertText;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button createButton;
        [SerializeField] private TMP_InputField loginIDInputField;
        [SerializeField] private TMP_InputField loginPWInputField;
        [SerializeField] private TMP_InputField createIDInputField;
        [SerializeField] private TMP_InputField createPWInputField;
        [SerializeField] private TMP_InputField createConfirmInputField;
        [SerializeField] private Color successColor;

        private void Start()
            {
                _imageUploader = GetComponent<ImageUploader>();
            }

        public void OnLoginClick()
            {
                ActivateButtons(false);
                StartCoroutine(TryLogin());
            }

        public void OnCreateClick()
            {
                ActivateButtons(false);
                StartCoroutine(TryCreate());
            }

        private IEnumerator TryLogin()
            {
                string username = loginIDInputField.text;
                string password = loginPWInputField.text;
                
                if (username.Length < 3 || username.Length > 14)
                    {
                        loginAlertText.text = "Invalid username";
                        ActivateButtons(true);
                        yield break;
                    }

                WWWForm form = new WWWForm();
                form.AddField("rUsername", username);
                form.AddField("rPassword", password);

                UnityWebRequest request = UnityWebRequest.Post(loginEndpoint, form);
                Debug.Log("Login Request sent to: " + loginEndpoint);
                Debug.Log(request);
                var handler = request.SendWebRequest();

                float startTime = 0.0f;
                while (!handler.isDone)
                    {
                        startTime += Time.deltaTime;

                        if (startTime > 10.0f)
                            {
                                break;
                            }

                        yield return null;
                    }
                
                if (request.result == UnityWebRequest.Result.Success)
                    {
                        LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                        if (response.code == 0) // login success?
                            {
                                string dbUserName = response.data.username;
                                string dbUserImage = response.data.userImage;
                                
                                // 여기서 추가적인 처리하면됨
                                // ActivateButtons(false);
                                GameManager.SetUserName(dbUserName);
                                loginAlertText.text = "Welcome";
                                loginAlertText.color = successColor;
                                
                                // Start a new game
                                menuManager.ApplyUserInfo(dbUserName, dbUserImage);
                                menuManager.OnConnectClick();
                            }
                        else
                            {
                                switch (response.code)
                                    {
                                        case 1:
                                            loginAlertText.text = "Invalid credentials";
                                            ActivateButtons(true);
                                            break;
                                        default:
                                            loginAlertText.text = "Corruption detected";
                                            ActivateButtons(false);
                                            break;
                                    }
                            }
                    }
                else
                    {
                        loginAlertText.text = "Error connecting to the server...";
                        ActivateButtons(true);
                    }


                yield return null;
            }

        private IEnumerator TryCreate()
            {
                string username = createIDInputField.text;
                string password = createPWInputField.text;
                string userImage;

                if (username.Length < 3 || username.Length > 24)
                    {
                        createAlertText.text = "Invalid ID";
                        ActivateButtons(true);
                        yield break;
                    }
                else if (createPWInputField.text != createConfirmInputField.text)
                    {
                        createAlertText.text = "Password Does not Match";
                        ActivateButtons(true);
                        yield break;
                    }
                
                // Get the base64 encoded image from the ImageUploader
                userImage = _imageUploader.GetBase64EncodedImage();
                if (string.IsNullOrEmpty(userImage))
                    {
                        createAlertText.text = "Please select an image.";
                        ActivateButtons(true);
                        yield break;
                    }

                WWWForm form = new WWWForm();
                form.AddField("rUsername", username);
                form.AddField("rPassword", password);
                form.AddField("userImage", userImage);
                
                UnityWebRequest request = UnityWebRequest.Post(createEndpoint, form);
                Debug.Log("Create Request sent to: " + loginEndpoint);
                Debug.Log(request);
                var handler = request.SendWebRequest();

                float startTime = 0.0f;
                while (!handler.isDone)
                    {
                        startTime += Time.deltaTime;

                        if (startTime > 10.0f)
                            {
                                break;
                            }

                        yield return null;
                    }

                if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log(request.downloadHandler.text);
                        CreateResponse response = JsonUtility.FromJson<CreateResponse>(request.downloadHandler.text);

                        if (response.code == 0)
                            {
                                createAlertText.text = "Account has been created!";
                                createAlertText.color = successColor;
                            }
                        else
                            {
                                switch (response.code)
                                    {
                                        case 1:
                                            createAlertText.text = "Invalid credentials";
                                            break;
                                        case 2:
                                            createAlertText.text = "Username is already taken";
                                            break;
                                        case 3:
                                            createAlertText.text = "Password is unsafe";
                                            break;
                                        default:
                                            createAlertText.text = "Corruption detected";
                                            break;
                                    }
                            }
                    }
                else
                    {
                        Debug.LogError("Request failed. Error: " + request.error);
                        Debug.LogError("Response code: " + request.responseCode);
                        createAlertText.text = "Error connecting to the server...";
                    }

                ActivateButtons(true);

                yield return null;
            }

        private void ActivateButtons(bool toggle)
            {
                loginButton.interactable = toggle;
                createButton.interactable = toggle;
            }
    }