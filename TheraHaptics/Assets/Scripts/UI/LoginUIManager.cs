using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;

public class LoginUIManager : MonoBehaviour
{

    // Getting input fields
    [SerializeField] private TMP_InputField userEmail;
    [SerializeField] private TMP_InputField userPassword;
    [SerializeField] private TMP_InputField productKey;
    [SerializeField] private TMP_InputField firstNameInput;
    [SerializeField] private TMP_InputField lastNameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField newPassInput;
    [SerializeField] private TMP_InputField confirmPassInput;
    [SerializeField] private Toggle showPassword;
    [SerializeField] private Toggle keepLoggedIn;
    
    // Get Error Message
    [SerializeField] private GameObject errorMessage;

    // Make Necessary Variables
    private string email;
    private string password;
    private string confirmPassword;
    private string firstName;
    private string lastName;
    private bool loginAccepted;

    private string regKey;

    public static string ProductKeyValue;
    public string productKeyId;
    private static readonly HttpClient client = new HttpClient();

    // Awake is called before the first frame update
    private void Awake()
    {
        loginAccepted = false;
    }

    public void ReturnToHome() {
        SceneManager.LoadScene(0);
    }

    // Method for the Show Password Toggle
    public void HideShowPassword(){
        // Get Toggle value
        bool isShown = showPassword.isOn;
        // Use toggle value to set is password is shown or not
        if(isShown){
            userPassword.contentType = TMP_InputField.ContentType.Standard;
        }
        else{
            userPassword.contentType = TMP_InputField.ContentType.Password;
        }
        // Update input field
        userPassword.ForceLabelUpdate();
    }

    // Method for pressing Sign In button
    public void SignIn(){
        // Deactivate error message
        errorMessage.SetActive(false);
        // Check is user wants to stay logged in
        if (keepLoggedIn.isOn){
            //Code to Stay Logged in (save user credentials)
        }
        else{
            
        }
        // Get text input field data
        email = userEmail.text;
        password = userPassword.text;

        // Insert Input Verification and then API here
        // return loginAccepted = true/false

        loginAccepted = false; // For Testing Only
        if (loginAccepted)
        {
            Debug.Log(email + ", " + password); // For Testing Only

            /* if (isPatient){
            Load Patient Account Scene
            }
            elseif (isDoctor){
            Load Doctor Account Scene
            }
            */
        }
        else{
            // Activate error message
            errorMessage.SetActive(true);
        }
    }
    public void RegisterAccount(){
        // Deactivate error message
        errorMessage.SetActive(false);
        // Get text input field data
        firstName = firstNameInput.text;
        lastName = lastNameInput.text;
        email = emailInput.text;
        password = newPassInput.text;
        confirmPassword = confirmPassInput.text;

        // Insert Input Verification and then API here
        // return loginAccepted = true/false

        loginAccepted = false; // For Testing Only
        if (loginAccepted)
        {
            Debug.Log(email + ", " + password); // For Testing Only

            /* if (isPatient){
            Load Patient Account Scene
            }
            elseif (isDoctor){
            Load Doctor Account Scene
            }
            */
        }
        else{
            // Activate error message
            errorMessage.SetActive(true);
        }
    }

public static string ProductKeyId; // To save the productKeyId from the response

public async void RegisterKey()
{
    regKey = productKey.text;
    string url = $"http://localhost:5089/api/productKey/{regKey}";

    try
    {
        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            var responseJson = JsonUtility.FromJson<ProductKeyResponse>(responseBody);
            ProductKeyId = responseJson.productKeyId; // Save productKeyId
            Debug.Log($"Saved ProductKeyId: {ProductKeyId}");
            SceneManager.LoadScene(2);
        }
        else
        {
            Debug.Log($"Error: {response.StatusCode}");
            if (errorMessage != null) errorMessage.SetActive(true);
        }
    }
    catch (HttpRequestException e)
    {
        Debug.LogError($"Request exception: {e.Message}");
        if (errorMessage != null) errorMessage.SetActive(true);
    }
}


[Serializable]
public class TherapistPayload
{
    public string firstName;
    public string lastName;
    public string emailAddress;
    public string productKeyId;
    public string userPw;
    public List<string> assignedPatients; // Include this to match Postman
}
/*public async void RegisterTherapist()
{
    // Hardcoded values for testing
    var payload = new TherapistPayload
    {
        firstName = "test",
        lastName = "test",
        emailAddress = "zaiyanazeem@gmail.com",
        productKeyId = "677353cdb9738f4068ce8ae2",
        userPw = "T@st12345"
    };

    // Serialize payload to JSON
    string jsonPayload = JsonUtility.ToJson(payload);
    Debug.Log($"Hardcoded Payload: {jsonPayload}");

    // API URL
    string url = "http://localhost:5089/api/user/therapist";

    using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Therapist registered successfully: {request.downloadHandler.text}");
            SceneManager.LoadScene(2); // Move to the next scene
        }
        else
        {
            Debug.LogError($"Registration failed: {request.error}");
            Debug.LogError($"Response: {request.downloadHandler.text}");
            //ShowErrorMessage($"Registration failed: {request.downloadHandler.text}");
        }
    }
}

/* public async void RegisterTherapist()
{
    // Validate input fields
    if (string.IsNullOrWhiteSpace(firstNameInput.text) ||
        string.IsNullOrWhiteSpace(lastNameInput.text) ||
        string.IsNullOrWhiteSpace(emailInput.text) ||
        string.IsNullOrWhiteSpace(ProductKeyId) ||
        string.IsNullOrWhiteSpace(userPassword.text)) 
    {
        ShowErrorMessage("All fields are required.");
        return;
    }

    // Create the payload
    var payload = new TherapistPayload
    {
        firstName = firstNameInput.text.Trim(),
        lastName = lastNameInput.text.Trim(),
        emailAddress = emailInput.text.Trim(),
        userPw = userPassword.text.Trim(),
        productKeyId = ProductKeyId,
        assignedPatients = new List<string>() // Always include this
    };

    // Serialize payload to JSON
    string jsonPayload = JsonUtility.ToJson(payload);
    Debug.Log($"Payload: {jsonPayload}");

    // API URL
    string url = "http://localhost:5089/api/user/therapist";

    using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        var operation = request.SendWebRequest();
        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Therapist registered successfully: {request.downloadHandler.text}");
            SceneManager.LoadScene(2); // Move to the next scene
        }
        else
        {
            Debug.LogError($"Registration failed: {request.error}");
            Debug.LogError($"Response: {request.downloadHandler.text}");
            ShowErrorMessage($"Registration failed: {request.downloadHandler.text}");
        }
    }
}

// Helper method to show error messages
private void ShowErrorMessage(string message)
{
    if (errorMessage != null)
    {
        errorMessage.SetActive(true);
        TMP_Text errorText = errorMessage.GetComponentInChildren<TMP_Text>();
        if (errorText != null)
        {
            errorText.text = message;
        }
    }
} */


    // Make sure to save values and move to next scene (index: 2)
/*
public async void RegisterKey()
    {
        // Get the key entered by the user
        regKey = productKey.text;

        // Define the API endpoint
        string url = $"http://localhost:5089/api/productKey/{regKey}";

        try
        {
            // Make the HTTP GET request
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                // Parse the response from the server
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.Log($"Response: {responseBody}");

                // Assuming the server response indicates success
                // Hide the error message (if it's currently displayed)
                if (errorMessage != null)
                {
                    errorMessage.SetActive(false);
                }
                ProductKeyValue = productKey.text;
                SceneManager.LoadScene(2);
            }
            else
            {
                // Handle the error case
                Debug.Log($"Error: {response.StatusCode}");

                // Show the error message
                if (errorMessage != null)
                {
                    errorMessage.SetActive(true);
                }
            }
        }
        catch (HttpRequestException e)
        {
            Debug.LogError($"Request exception: {e.Message}");

            // Show the error message in case of request failure
            if (errorMessage != null)
            {
                errorMessage.SetActive(true);
            }
        }
    }

public async void RegisterTherapist()
{
    string retrievedProductKey = LoginUIManager.ProductKeyValue;
    // Deactivate the error message initially
    errorMessage.SetActive(false);
    
    // Collect input data
    firstName = firstNameInput.text;
    lastName = lastNameInput.text;
    email = emailInput.text;
    password = newPassInput.text;
    confirmPassword = confirmPassInput.text;
    regKey = retrievedProductKey;

    // Validate inputs
    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
        string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
        string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(regKey))
    {
        ShowErrorMessage("All fields are required.");
        return;
    }

    if (password != confirmPassword)
    {
        ShowErrorMessage("Passwords do not match.");
        return;
    }

    // Create payload
    var payload = new
    {
        firstName = firstName,
        lastName = lastName,
        emailAddress = email,
        password = password,
        productKeyId = regKey
    };

    // Serialize payload to JSON
    string jsonPayload = JsonUtility.ToJson(payload);

    // API URL
    string url = "http://localhost:5089/api/user/therapist";

    using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
    {
        // Set request content
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send request
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Therapist registered successfully: {request.downloadHandler.text}");
            SceneManager.LoadScene(2); // Move to the next scene
        }
        else
        {
            Debug.LogError($"Registration failed: {request.error}");
            ShowErrorMessage("Registration failed. Please check your inputs.");
        }
    }
}

// Helper method to show error messages
private void ShowErrorMessage(string message)
{
    if (errorMessage != null)
    {
        errorMessage.SetActive(true);
        TMP_Text errorText = errorMessage.GetComponentInChildren<TMP_Text>();
        if (errorText != null)
        {
            errorText.text = message;
        }
    }
}
*/

    // Load Forgot Password Scene
    public void ForgotPassword(){
       
    }

    // Load Sign Up Scene starting with Key Registration
    public void SignUp(){
        SceneManager.LoadScene(1);
    }

    public void SendVerificationEmail(){
        email = emailInput.text;
        // Insert Input Verification and then API here

    }
    public void ChangePassword(){
        password = newPassInput.text;
        confirmPassword = confirmPassInput.text;
        // Insert Input Verification and then API here

    }
}

[Serializable]
public class ProductKeyResponse
{
    public string productKeyId;
}

[Serializable]
public class ProductKeyResponse
{
    public string productKeyId;
}
