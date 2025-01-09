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

    // Make sure to save values and move to next scene (index: 2)
    public async void RegisterKey(){
        regKey = productKey.text;
        string url = $"http://localhost:5089/api/productKey/{regKey}";

        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            Debug.Log($"Response: {responseBody}");
        }
        else
        {
            Debug.Log($"Error: {response.StatusCode}");
        }
    }
    
    // Load Forgot Password Scene
    public void ForgotPassword(){
        SceneManager.LoadScene(3);
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
