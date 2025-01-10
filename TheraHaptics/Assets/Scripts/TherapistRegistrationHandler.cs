using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class TherapistRegistrationHandler : MonoBehaviour
{
    public static TherapistRegistrationHandler Instance; // Singleton for global access

    public static string ProductKeyId; // Store ProductKeyId globally

    [SerializeField] private TMP_InputField firstNameInput; // First Name
    [SerializeField] private TMP_InputField lastNameInput; // Last Name
    [SerializeField] private TMP_InputField emailInput; // Email
    [SerializeField] private TMP_InputField passwordInput; // Password
    [SerializeField] private GameObject errorMessage; // Error message display

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to fetch product key (should have been called in the previous scene)
    public static void SetProductKeyId(string productKeyId)
    {
        ProductKeyId = productKeyId;
        Debug.Log($"ProductKeyId set: {ProductKeyId}");
    }

    // Method to register therapist
    public async Task<bool> RegisterTherapist()
    {
        if (string.IsNullOrWhiteSpace(ProductKeyId))
        {
            ShowErrorMessage("Product Key ID is missing. Please go back and enter a valid product key.");
            return false;
        }

        string firstName = firstNameInput.text;
        string lastName = lastNameInput.text;
        string emailAddress = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(emailAddress) ||
            string.IsNullOrWhiteSpace(password))
        {
            ShowErrorMessage("All fields are required.");
            return false;
        }

        // Payload for API
        TherapistPayload payload = new TherapistPayload
        {
            firstName = firstName.Trim(),
            lastName = lastName.Trim(),
            emailAddress = emailAddress.Trim(),
            productKeyId = ProductKeyId,
            assignedPatients = new List<string>(), // Include as required
            password = password.Trim()
        };

        // Serialize to JSON
        string jsonPayload = JsonUtility.ToJson(payload);
        string url = "http://localhost:5089/api/user/therapist";

        try
        {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"Therapist registered successfully: {request.downloadHandler.text}");
                    SceneManager.LoadScene(3);
                    return true;
                }
                else
                {
                    Debug.LogError($"Registration failed: {request.error}");
                    Debug.LogError($"Response: {request.downloadHandler.text}");
                    ShowErrorMessage("Registration failed. Please check your inputs.");
                    return false;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception occurred: {e.Message}");
            ShowErrorMessage("Failed to connect to the server.");
            return false;
        }
    }

    // Helper method to show error messages
    private void ShowErrorMessage(string message)
    {
        if (errorMessage != null)
        {
            errorMessage.SetActive(true);
            var errorText = errorMessage.GetComponentInChildren<TMPro.TMP_Text>();
            if (errorText != null)
            {
                errorText.text = message;
            }
        }
    }
}

[System.Serializable]
public class TherapistPayload
{
    public string firstName;
    public string lastName;
    public string emailAddress;
    public string productKeyId;
    public List<string> assignedPatients;
    public string password;
}