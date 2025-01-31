using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class LoginView : MonoBehaviour
{
    // info input fields
    [SerializeField] private TMP_InputField userEmail;
    [SerializeField] private TMP_InputField userPassword;
    [SerializeField] private TMP_InputField confirmPassword;
    [SerializeField] private TMP_InputField productKey;
    [SerializeField] private TMP_InputField firstNameInput;
    [SerializeField] private TMP_InputField lastNameInput;

    // Toggles
    [SerializeField] private Toggle keepLoggedIn; // Functionality not done yet
    [SerializeField] private Toggle showPassword;

    // Buttons
    [SerializeField] private Button submitProductKeyButton;

    // Accessing Error Message object and Component
    [SerializeField] private GameObject errorMessage;
    [SerializeField] private TMP_Text errorMessageText;

    // Constructor
    public LoginView(){

    }

    // ==========Common Methods===========
    // Method for the Show Password Toggles
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
    public void HandleGenericFail (String errorMessageTextReturn){
        errorMessageText.text = errorMessageTextReturn;
        errorMessage.SetActive(true);
    }

    // Method for Pressing the Return Home Link
    public void ReturnHomePressed(){
        SceneManager.LoadScene("SignIn");
    }

    // ==========SignIn Scene Methods==========
    // Method for sign in Button on SignIn Scene
    public void SignInButtonClicked () {
        LoginController.SignIn(userEmail.text, userPassword.text);
    }

    // Method for Failed Sign In
    public void HandleSignInError(String errorMessageTextReturn){
        // Debug.Log("this was run");
        userPassword.text = "";
        userPassword.ForceLabelUpdate();
        errorMessageText.text = errorMessageTextReturn;
        errorMessage.SetActive(true);
    }

    // Method for Successful Sign In
    public void HandleSignInSuccess (bool userType){
        if(userType == false){
            SceneManager.LoadScene("TherapistDashboard");
        }
        else{
            SceneManager.LoadScene("PatientDashboard");
        }
    }

    // Method for Pressing the Forgot Password Link on the SignIn Scene
    public void ForgotPasswordPressed(){
        // LoginController.xyz(userEmail.text);
        SceneManager.LoadScene("ForgotPassword1");
    }

    // Method for Pressing the Sign Up as new Physiotherapist Link on the SignIn Scene
    public void SignUpPressed(){
        SceneManager.LoadScene("RegisterSerialKey");
    }

    public void ProductKeySubmitPressed(){
        //LoginController.xyz(productKey.text)
        Debug.Log("Submit Product Key Pressed");
    }
    public void IsProductKeyLongEnough (){
        if(productKey.text.Length == 8){
            submitProductKeyButton.interactable = true;
        }
        else if (productKey.text.Length > 8){
            productKey.text = productKey.text.Substring(0, 8);
        }
        else{
            submitProductKeyButton.interactable = false;
        }
    }

    public void HandleProductKeySuccess (){
        SceneManager.LoadScene("RegisterAccount");
    }

    public void SendEmailPressed(){
        Debug.Log("Send Email Pressed");
        // LoginController.xyz(userEmail.text);
    }
    public void HandleForgotPassSubmitSuccess(){
        SceneManager.LoadScene("ForgotPassword2");
    }

    public void SaveNewPasswordPressed(){
        Debug.Log("Save Password Pressed");
        // LoginController.xyz(userPassword.text, confirmPassword.text);
    }

    public void HandleSavePasswordSuccess(){
        SceneManager.LoadScene("SignIn");
    }

    public void RegisterSubmitPressed(){
        Debug.Log("Register Submit Pressed");
        // LoginController.xyz(firstNameInput.text, lastNameInput.text, userEmail.text, userPassword.text, confirmPassword.text);
    }
    public void HandleRegisterSuccess(bool userType){
        if(userType == false){
            SceneManager.LoadScene("TherapistDashboard");
        }
        else{
            SceneManager.LoadScene("PatientDashboard");
        }
    }
}
