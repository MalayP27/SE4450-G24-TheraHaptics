using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class SignIn : MonoBehaviour
{
    public TMP_InputField emailInput; // TMP Input for Email
    public TMP_InputField passwordInput; // TMP Input for Password
    public Toggle showPasswordToggle; // Toggle to show/hide password
    public Button signInButton; // Button to trigger sign-in
    public Toggle keepLoggedInToggle; // Checkbox for "Keep me logged in"

    private const string ApiUrl = "http://localhost:5089/api/user/login"; // Replace with your actual API endpoint

    void Start()
    {
        // Set up button listener
        signInButton.onClick.AddListener(OnSignIn);
        
        // Set up show password toggle
        showPasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility);
    }

    void TogglePasswordVisibility(bool isVisible)
    {
        passwordInput.contentType = isVisible ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate(); // Force the input field to update its content type
    }

    void OnSignIn()
    {
        // Gather input field values
        string email = emailInput.text;
        string password = passwordInput.text;

        // Validate fields
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Email and password must be filled!");
            return;
        }

        // Create login data
        LoginDto loginData = new LoginDto
        {
            emailAddress = email,
            password = password
        };

        // Send the POST request
        StartCoroutine(PostSignIn(loginData));
    }

    IEnumerator PostSignIn(LoginDto loginData)
    {
        string jsonData = JsonUtility.ToJson(loginData);
        using (UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Sign-in successful!");
                Debug.Log($"Response: {request.downloadHandler.text}");
                // You can parse and use the response, e.g., storing the JWT token if needed
                SceneManager.LoadScene(7);
            }
            else
            {
                Debug.LogError($"Sign-in failed: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
            }
        }
    }
}

// Define the DTO class for sending login data
[System.Serializable]
public class LoginDto
{
    public string emailAddress;
    public string password;
}