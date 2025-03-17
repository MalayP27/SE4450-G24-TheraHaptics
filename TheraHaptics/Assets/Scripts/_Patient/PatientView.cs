using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PatientView : MonoBehaviour
{

    // Temp Variables
    string tempString = "Placeholder";
    
    // Account info text boxes
    [Header("Account Boxes")]
    [SerializeField] private TMP_Text nameDataBox;
    [SerializeField] private TMP_Text genderDataBox;
    [SerializeField] private TMP_Text dobDataBox;
    [SerializeField] private TMP_Text phoneDataBox;
    [SerializeField] private TMP_Text emailDataBox;
    [SerializeField] private TMP_Text addressDataBox;

    [Header("Misc")]
    // Text Boxes
    [SerializeField] private TMP_Text helloMessage;

    // Constructor
    public PatientView(){

    }

    // ==========Awake Method==========
    public void Awake()
    {
        // Find out which scene I'm on to awaken that scene
        Scene currentScene = SceneManager.GetActiveScene ();
        if (currentScene.name=="PatientAccount"){
            Debug.Log("This is the PatientAccount Scene");
            FillName(tempString, 1/*Zaiyan's Return Therapist Name Function*/);
            FillGender(tempString/*Zaiyan's Return Therapist Name Function*/);
            FillDob(tempString/*Zaiyan's Return Therapist DOB Function*/);
            FillPhoneNum(tempString/*Zaiyan's Return Therapist PhoneNum Function*/);
            FillEmail(tempString/*Zaiyan's Return Therapist Email Function*/);
            FillAddress(tempString/*Zaiyan's Return Therapist Address Function*/);
        }
    }
    // ==========Common Methods===========
    // Method to update Main Welcome Message
    public void setWelcomeMessage(String helloMessageReturn){
        helloMessage.text = helloMessageReturn;
    }

    // Methods For SideBar Buttons
    // Method for "My DashBoard" Button click
    public void PatientDashboard(){
        SceneManager.LoadScene("PatientDashboard");
    }

    // Method for "My Reports" Button click
    public void PatientReports(){
        SceneManager.LoadScene("PatientReports");
    }

    // Method for "My Account" Button click
    public void PatientAccount(){
        SceneManager.LoadScene("PatientAccount");
    }

    // Method for "Support" Button click
    public void PatientSupport(){
        SceneManager.LoadScene("Support");
    }

    // Method for "Log Out" Button click
    public void PatientLogOut(){
        SceneManager.LoadScene("SignIn");
    }

    // ==========PatientDashboard Scene Methods===========
    // Method to populate Exercise Plan Box
    public void populateExercisePlan(String[] exercises){
        
    }

    // Method to populate Goal Box
    public void populateGoalBox(){
        
    }
    
    // Method to create new Goal
    public void createNewGoal(){
        
    }

    // Method to Fill Progress Bars
    public void fillProgressBar(double percentageFilled){
        
    }

    // ==========PatientAccount Scene Methods==========
    // Methods for Populating info boxes
    public void FillName(string name, int type){
        if (type == 0){
            nameDataBox.text = "<cspace=-2>Patient: " + name;
        }
        else{
            nameDataBox.text = "<cspace=-2>Dr.\n<cspace=-2>" + name;
        }
    }
    public void FillGender(string gender){
        genderDataBox.text = gender;
    }
    public void FillDob(string dob){
        dobDataBox.text = dob;
    }
    public void FillPhoneNum(string phoneNum){
        phoneDataBox.text = phoneNum;
    }
    public void FillEmail(string email){
        emailDataBox.text = email;
    }
    public void FillAddress(string address){
        addressDataBox.text = address;
    }

    // Method for when the Therapist wants to change their password
    public void PatientChangePassword(){
        SceneManager.LoadScene("ForgotPassword1");
    }
}
