using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterController : MonoBehaviour
{
    // Reuse the LoginModel and LoginView from your other scripts.
    public static LoginModel loginModel = new LoginModel();
    [SerializeField] public static LoginView loginView = new LoginView();

    // Static variable to store the therapistId
    public static string TherapistId { get; set;}

    // This class is used to deserialize the GET response from the product key verification endpoint.
    [Serializable]
    public class ProductKeyResponse
    {
        public string productKeyId;
    }

    // (Optional) This class is used to deserialize the POST response from the registration endpoint.
    // Note: In your UserController, the [HttpPost("therapist")] method returns Ok(therapist)
    // after generating the JWT token and setting the cookie.
    // You may choose to deserialize the response if needed.
    [Serializable]
    public class RegisterResponse
    {
        public string therapistId;  // The therapist ID returned by the API
        public string token;        // The token returned by the API (if any)
        public string role;         // Expected to be "therapist" (if provided)
    }

    private void Awake()
    {
        // Assume that the LoginView component is attached to the same GameObject.
        loginView = GetComponent<LoginView>();
    }

    public static async void VerifyProductKey(string productKeyInput)
    {
        if (string.IsNullOrEmpty(productKeyInput))
        {
            loginView.HandleGenericFail("Product key cannot be empty.");
            return;
        }

        // Build the URL using the provided product key.
        string url = "http://localhost:5089/api/productKey/" + productKeyInput;

        using (var client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ProductKeyResponse keyResponse = JsonUtility.FromJson<ProductKeyResponse>(responseBody);
                    
                    // Store the productKeyId in the model for later use during registration.
                    loginModel.SetProductKey(keyResponse.productKeyId);
                    
                    // Notify the view that the product key has been successfully verified.
                    loginView.HandleProductKeySuccess();
                }
                else
                {
                    loginView.HandleGenericFail("Product key error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                loginView.HandleGenericFail("Error verifying product key: " + ex.Message);
            }
        }
    }

    public static async void Register(string firstName, string lastName, string email, string password, string confirmPassword)
    {
        // Input validation: Ensure all fields are provided.
        if (string.IsNullOrEmpty(firstName) ||
            string.IsNullOrEmpty(lastName) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(confirmPassword))
        {
            loginView.HandleGenericFail("All fields are required.");
            return;
        }

        // Ensure the password and confirmation match.
        if (password != confirmPassword)
        {
            loginView.HandleGenericFail("Passwords do not match.");
            return;
        }

        // Retrieve the productKeyId stored from the product key verification step.
        string productKeyId = loginModel.GetSProductKey();
        if (string.IsNullOrEmpty(productKeyId))
        {
            loginView.HandleGenericFail("Product key is missing or not verified.");
            return;
        }

        // Build the JSON payload matching the TherapistPostDto expected by the API.
        string jsonPayload = "{\"firstName\":\"" + firstName +
                             "\",\"lastName\":\"" + lastName +
                             "\",\"emailAddress\":\"" + email +
                             "\",\"password\":\"" + password +
                             "\",\"productKeyId\":\"" + productKeyId + "\"}";

        Debug.Log("Register JSON Payload: " + jsonPayload);

        using (var client = new HttpClient())
        {
            try
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("http://localhost:5089/api/user/therapist", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    RegisterResponse registerResponse = JsonUtility.FromJson<RegisterResponse>(responseBody);

                    // Assuming the response contains a therapistId field
                    TherapistId = registerResponse.therapistId;
                    Debug.Log("Therapist ID: " + TherapistId);

                    loginView.HandleRegisterSuccess();
                }
                else
                {
                    loginView.HandleGenericFail("Registration failed: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                loginView.HandleGenericFail("Error during registration: " + ex.Message);
            }
        }
    }
}
