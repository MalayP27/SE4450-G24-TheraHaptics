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
    public void HandleSignInSuccess (string userType){
        if(userType == "therapist"){
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
        SceneManager.LoadScene("RegisterProductKey");
    }

    // ==========ProductKey Scene Methods==========
    // Method for Pressing the Submit Button on the ProductKeyScene
    public void ProductKeySubmitPressed(){
        RegisterController.VerifyProductKey(productKey.text);
        Debug.Log("Submit Product Key Pressed");
    }

    // Method for activating and deactivating button if product key is not exactly 8 characters long
    public void IsProductKeyLongEnough (){
        // Activates button on exactly 8 characters
        if(productKey.text.Length == 8){
            submitProductKeyButton.interactable = true;
        }
        // Limits the Length of the Input and doesn't allow any more characters to be put in
        else if (productKey.text.Length > 8){
            productKey.text = productKey.text.Substring(0, 8);
        }
        // Makes button uninteractable if length is less than 8
        else{
            submitProductKeyButton.interactable = false;
        }
    }

    // Method for successful product key submission and moves to the Account creation Page
    public void HandleProductKeySuccess (){
        SceneManager.LoadScene("RegisterAccount");
    }

    // ==========Password recovery send email Scene Methods==========
    // Method for when the send email button is pressed on the ForgotPassword1 Scene
    public void SendEmailPressed(){
        Debug.Log("Send Email Pressed");
        LoginController.ForgotPassword(userEmail.text);

    }

    // Method for when the email is sent correctly to go to the next step
    public void HandleForgotPassSubmitSuccess(){
        SceneManager.LoadScene("ForgotPassword2");
    }

    // ==========Create new password Scene Methods==========
    // Method for when the save new Password button is pressed on the ForgotPassword2 Scene
    public void SaveNewPasswordPressed(){
        Debug.Log("Save Password Pressed");
        LoginController.ChangePassword(userPassword.text, confirmPassword.text);
    }

    // Method for new password saved successfully
    public void HandleSavePasswordSuccess(){
        SceneManager.LoadScene("SignIn");
    }

    // ==========Register New Account Scene methods==========
    // Method for when the Register button is pressed on the RegisterAccount Scene
    public void RegisterSubmitPressed(){
        Debug.Log("Register Submit Pressed");
        // Call the Register method in RegisterController with the input values.
    RegisterController.Register(
        firstNameInput.text,
        lastNameInput.text,
        userEmail.text,
        userPassword.text,
        confirmPassword.text
    );
}

    // Method for Handling a successful account Registration
    public void HandleRegisterSuccess(){
            SceneManager.LoadScene("TherapistDashboard");
    }
}
