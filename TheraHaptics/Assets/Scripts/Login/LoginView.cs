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
        LoginController.SignIn(userEmail.text, userPassword.text);
    }
    public void HandleSignInError(String errorMessageTextReturn){
        // Debug.Log("this was run");
        userPassword.text = "";
        userPassword.ForceLabelUpdate();
        errorMessageText.text = errorMessageTextReturn;
        errorMessage.SetActive(true);
    }

    public void HandleSignInSuccess (bool userType){
        if(userType == false){
            SceneManager.LoadScene("TherapistDashboard");
        }
        else{
            SceneManager.LoadScene("PatientDashboard");
        }
    }

    public void ForgotPasswordPressed(){
        // LoginController.xyz(userEmail.text);
        SceneManager.LoadScene("ForgotPassword1");
    }
    public void SignUpPressed(){
        SceneManager.LoadScene("RegisterSerialKey");
    }
    public void ReturnHomePressed(){
        SceneManager.LoadScene("SignIn");
    }
    public void ProductKeySubmitPressed(){
        //LoginController.xyz(productKey.text)
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
    public void HandleProductKeyFail (String errorMessageTextReturn){
        errorMessageText.text = errorMessageTextReturn;
        errorMessage.SetActive(true);
    }
    public void HandleProductKeySuccess (){
        SceneManager.LoadScene("RegisterAccount");
    }
}
