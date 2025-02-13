using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

public class LoginController : MonoBehaviour
{
    public static LoginModel loginModel = new LoginModel();
    [SerializeField] public static LoginView loginView = new LoginView();

    // Response model for login
    [Serializable]
    public class LoginResponse
    {
        public string token;  // The token returned by the API.
        public string role;   // The role returned by the API (e.g., "patient" or "therapist")
    }

    // DTO class for Forgot Password payload
    [Serializable]
    public class ForgotPasswordDto
    {
        public string EmailAddress;
    }

    private void Awake()
    {
        // Assume that the LoginView component is attached to the same GameObject.
        loginView = GetComponent<LoginView>();
    }

    /// Attempts to sign in the user using the provided email and password.
    public static async void SignIn(string email, string password)
    {
        // Input validation: Check for empty strings.
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            loginView.HandleSignInError("Email and password cannot be empty.");
            return;
        }

        // Create the JSON payload with the correct property names.
        string jsonPayload = "{\"emailAddress\":\"" + email + "\",\"password\":\"" + password + "\"}";

        Debug.Log("JSON Payload: " + jsonPayload); // Debug the payload to confirm it's correct.

        // Use HttpClient to send the POST request.
        using (var client = new HttpClient())
        {
            try
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("http://localhost:5089/api/user/login", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseBody);
                    loginView.HandleSignInSuccess(loginResponse.role);
                }
                else
                {
                    loginView.HandleSignInError("Login failed: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                loginView.HandleSignInError("Error during login: " + ex.Message);
            }
        }
    }

    /// Sends a forgot password request to the API using the provided email address. (LoginView.cs SendEmailPressed button)
    public static async void ForgotPassword(string email)
    {
        // Validate the email field.
        if (string.IsNullOrEmpty(email))
        {
            loginView.HandleSignInError("Email field must be filled!");
            return;
        }

        // Create the forgot password DTO and convert it to JSON.
        ForgotPasswordDto dto = new ForgotPasswordDto();
        dto.EmailAddress = email;
        string jsonData = JsonUtility.ToJson(dto);

        using (var client = new HttpClient())
        {
            try
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("http://localhost:5089/api/user/forgotPassword", content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Forgot password request successful!");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("Response: " + responseBody);
                }
                else
                {
                    Debug.LogError("Forgot password request failed: " + response.ReasonPhrase);
                    loginView.HandleSignInError("Forgot password request failed: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error during forgot password request: " + ex.Message);
                loginView.HandleSignInError("Error during forgot password request: " + ex.Message);
            }
        }
    }
}
