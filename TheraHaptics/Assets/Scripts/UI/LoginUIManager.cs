using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUIManager : MonoBehaviour
{

    // Getting input fields
    [SerializeField] private TMP_InputField userEmail;
    [SerializeField] private TMP_InputField userPassword;
    [SerializeField] private Toggle showPassword;
    [SerializeField] private Toggle keepLoggedIn;
    
    // Get Error Message
    [SerializeField] private GameObject errorMessage;

    // Make Necessary Variables
    private string email;
    private string password;
    private bool loginAccepted;

    // Awake is called before the first frame update
    private void Awake()
    {
        loginAccepted = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void ForgotPassword(){
        // Load Forgot Password Scene
    }

    public void SignUp(){
        // Load Sign Up Scene starting with Key Registration
    }

}
