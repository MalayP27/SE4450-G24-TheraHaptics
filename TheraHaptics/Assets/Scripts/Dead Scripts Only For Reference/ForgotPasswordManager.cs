using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro
using System.Collections;
using System.Text;
using UnityEngine.Networking;

public class ForgotPasswordManager : MonoBehaviour
{
    public GameObject forgotPasswordModal; // Reference to the ForgotPasswordModal canvas
    public Button forgotPasswordButton; // Reference to the "Forgot Password?" button
    public TMP_InputField emailInput; // TMP InputField for the email
    public Button submitButton; // Submit button inside the modal

    private const string ApiUrl = "http://localhost:5089/api/user/forgotPassword"; // Replace with the actual API endpoint

    void Start()
    {
        // Set up button listeners
        forgotPasswordButton.onClick.AddListener(ToggleForgotPasswordModal);
        submitButton.onClick.AddListener(OnForgotPasswordSubmit);
    }

    void ToggleForgotPasswordModal()
    {
        // Toggle modal visibility
        forgotPasswordModal.SetActive(!forgotPasswordModal.activeSelf);
    }

    void OnForgotPasswordSubmit()
    {
        // Gather email input value
        string email = emailInput.text;

        // Validate email field
        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("Email field must be filled!");
            return;
        }

        // Send the POST request
        StartCoroutine(PostForgotPassword(email));
    }

    IEnumerator PostForgotPassword(string email)
    {
        // Create the request payload
        var requestPayload = new ForgotPasswordDto
        {
            EmailAddress = email
        };
        string jsonData = JsonUtility.ToJson(requestPayload);

        using (UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Forgot password request successful!");
                Debug.Log($"Response: {request.downloadHandler.text}");
                // Optionally, hide the modal or show a success message
                forgotPasswordModal.SetActive(false);
            }
            else
            {
                Debug.LogError($"Forgot password request failed: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
            }
        }
    }
}

// DTO class for Forgot Password payload
[System.Serializable]
public class ForgotPasswordDto
{
    public string EmailAddress;
}
