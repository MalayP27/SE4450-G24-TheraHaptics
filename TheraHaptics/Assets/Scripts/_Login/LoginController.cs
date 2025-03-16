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
        public string token;              // The token returned by the API.
        public string role;               // The role returned by the API (e.g., "patient" or "therapist")
        public bool isTemporaryPassword;  // Indicates whether the user has a temporary password.
        public string therapistId;        // The therapist ID returned by the API (if role is "therapist")
    }

    // DTO class for Forgot Password payload (note: field names match server expectation)
    [Serializable]
    public class ForgotPasswordDto
    {
        public string emailAddress;
        public string tempPassword;
        public string newPassword;
    }

    // DTO class for Change Password payload
    [Serializable]
    public class ChangePasswordDto
    {
        public string emailAddress;
        public string tempPassword;
        public string newPassword;
    }

    private void Awake()
    {
        // Assume that the LoginView component is attached to the same GameObject.
        loginView = GetComponent<LoginView>();
    }

    /// Attempts to sign in the user using the provided email and password.
    /// If the user has a temporary password, navigates to the reset password scene.
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
        Debug.Log("JSON Payload: " + jsonPayload);

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
                    Debug.Log("isTemporaryPassword: " + loginResponse.isTemporaryPassword);

                    if (loginResponse.isTemporaryPassword)
                    {
                        // Navigate to the password reset scene (ForgotPassword2)
                        UnityEngine.SceneManagement.SceneManager.LoadScene("ForgotPassword2");
                    }
                    else
                    {
                        // Store therapistId if the role is "therapist"
                        if (loginResponse.role == "therapist")                        
                        {
                            RegisterController.TherapistId = loginResponse.therapistId;
                            Debug.Log("Therapist ID: " + RegisterController.TherapistId);
                        }

                        // Continue with the normal sign-in flow.
                        loginView.HandleSignInSuccess(loginResponse.role);
                    }
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

    /// Sends a forgot password request to the API using the provided email address.
    public static async void ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            loginView.HandleSignInError("Email field must be filled!");
            return;
        }

        // Create the forgot password DTO and convert it to JSON.
        ForgotPasswordDto dto = new ForgotPasswordDto { emailAddress = email };
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

    /// Sends a change password request to the API using the provided email, temporary password, and new password.
    public static async void ChangePassword(string email, string tempPassword, string newPassword)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(tempPassword) || string.IsNullOrEmpty(newPassword))
        {
            loginView.HandleSignInError("All fields are required for changing password.");
            return;
        }

        // Create the ChangePassword DTO and convert it to JSON.
        ChangePasswordDto dto = new ChangePasswordDto
        {
            emailAddress = email,
            tempPassword = tempPassword,
            newPassword = newPassword
        };
        string jsonPayload = JsonUtility.ToJson(dto);
        Debug.Log("ChangePassword JSON Payload: " + jsonPayload);

        using (var client = new HttpClient())
        {
            try
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync("http://localhost:5089/api/user/changePassword", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("Change password successful. Response: " + responseBody);
                    loginView.HandleSavePasswordSuccess();
                }
                else
                {
                    loginView.HandleSignInError("Change password failed: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                loginView.HandleSignInError("Error during change password: " + ex.Message);
            }
        }
    }
}