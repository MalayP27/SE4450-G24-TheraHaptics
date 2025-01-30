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
    [SerializeField] private TMP_InputField serialKey;
    [SerializeField] private TMP_InputField firstNameInput;
    [SerializeField] private TMP_InputField lastNameInput;

    // Toggles
    [SerializeField] private Toggle keepLoggedIn; // Functionality not done yet
    [SerializeField] private Toggle showPassword;

    // Accessing Error Message object and Component
    [SerializeField] private GameObject errorMessage;
    [SerializeField] private TMP_Text errorMessageText;

    // Model Class
    public static LoginModel loginModel = new LoginModel();

    public LoginView(){

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

    // Method for sign in Button on SignInScene
    public void SignInButtonClicked () {
        loginModel.SetEmail(userEmail.text);
        loginModel.SetPassword(userPassword.text);
        LoginController.SignIn(loginModel);
    }
    public void HandleError(String errorMessageTextReturn){
        Debug.Log("this was run");
        userPassword.text = "";
        userPassword.ForceLabelUpdate();
        errorMessageText.text = errorMessageTextReturn;
        errorMessage.SetActive(true);
    }

    public void HandleSignInSuccess (bool userType){
        if(userType == false){
            SceneManager.LoadScene("PhysioDashboard");
        }
        else{
            SceneManager.LoadScene("PatientDashboard");
        }
    }

    public void ForgotPasswordPressed(){
        SceneManager.LoadScene("ForgotPassword1");
    }
    public void SignUpPressed(){
        SceneManager.LoadScene("RegisterSerialKey");
    }
}
