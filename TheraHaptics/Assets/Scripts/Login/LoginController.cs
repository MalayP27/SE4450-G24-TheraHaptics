using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq; //input sanitization (.Any)

public class LoginController : MonoBehaviour
{

    public static LoginModel loginModel = new LoginModel();
    [SerializeField] public static LoginView loginView = new LoginView();
    // Start is called before the first frame update
    private void Awake()
    {
        loginView = GetComponent<LoginView>();
    }

    public static void SignIn(string email, string password)
    {
        // Input Validation
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            loginView.HandleSignInError("Email and password cannot be empty.");
            return;
        }

        if (!IsValidEmail(email))
        {
            loginView.HandleSignInError("Invalid email format.");
            return;
        }

        if (!IsStrongPassword(password))
        {
            loginView.HandleSignInError("Password must be at least 8 characters, contain an uppercase letter, a lowercase letter, a digit, and a special character.");
            return;
        }

        // Send API request
        Instance.StartCoroutine(SendSignInRequest(email, password));
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email && email.Contains("@");
        }
        catch
        {
            return false;
        }
    }

    private static bool IsStrongPassword(string password)
    {
        return password.Length >= 8 &&
            password.Any(char.IsUpper) && // Checks if password contains at least one uppercase letter
            password.Any(char.IsLower) && // Checks if password contains at least one lowercase letter
            password.Any(char.IsDigit); // Checks if password contains at least one digit
           // password.Any(ch => !char.IsLetterOrDigit(ch)); // At least one special character
    }

    private static IEnumerator SendSignInRequest(string email, string password)
    {
        string url = "http://localhost:5089/api/user/login";
        string jsonPayload = JsonUtility.ToJson(new LoginRequest(email, password));

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                loginView.HandleSignInError("Login failed: " + request.error);
            }
            else
            {
                // Parse response (assuming JSON)
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                if (!string.IsNullOrEmpty(response.Token))
                {
                    bool isPatient = response.Role.ToLower() == "patient";
                    loginView.HandleSignInSuccess(isPatient);
                }
                else
                {
                    loginView.HandleSignInError("Invalid email or password.");
                }
            }
        }
    }

    // Singleton instance for coroutine
    private static LoginController _instance;
    public static LoginController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LoginController>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("LoginController");
                    _instance = obj.AddComponent<LoginController>();
                }
            }
            return _instance;
        }
    }
}

// Classes for JSON serialization
[Serializable]
public class LoginRequest
{
    public string emailAddress;
    public string password;

    public LoginRequest(string email, string pass)
    {
        emailAddress = email;
        password = pass;
    }
}

[Serializable]
public class LoginResponse
{
    public string Token;
    public string Role;
}


