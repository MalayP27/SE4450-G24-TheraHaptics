using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class TherapistView : MonoBehaviour
{

    // Temp Variables
    string tempString = "Placeholder";
    static int tempPatientList = 8;
    string[] tempNames = new string[tempPatientList];
    string[] tempDates = new string[tempPatientList];
    string[] tempIDs = new string[tempPatientList];
    float[] tempProgress = new float[tempPatientList];

    // Account info text boxes
    [Header("Therapist Account Boxes")]
    [SerializeField] private TMP_Text nameDataBox;
    [SerializeField] private TMP_Text dobDataBox;
    [SerializeField] private TMP_Text phoneDataBox;
    [SerializeField] private TMP_Text emailDataBox;
    [SerializeField] private TMP_Text addressDataBox;
    
    // Patient text boxes
    [Header("All Patient Object lists")]
    [SerializeField] private TMP_Text[] patientNames;
    [SerializeField] private TMP_Text[] patientJoinDates;
    [SerializeField] private GameObject[] patientProgressBars;
    [SerializeField] private Image[] patientProgressBarFills;
    [SerializeField] private GameObject[] patientContactButtons;

    [Header("Add Patient Menu")]
    [SerializeField] private GameObject addPatientScreen;
    [SerializeField] private TMP_InputField firstName;
    [SerializeField] private TMP_InputField lastName;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField diagnosis;
    [SerializeField] private Button addPatientButton; // Added from TherapistController
    [SerializeField] private Button closeButton; // Added from TherapistController

    [Header("Misc")]
    [SerializeField] private TMP_Text helloMessage;
    [SerializeField] private TMP_InputField searchBar;
    
    int listLength;


    // Array for Holding which patients are in slots 1 to x
    private string[] patientIDs = new string[10];

    // Constructor
    public TherapistView(){

    }

    // ==========Awake Method==========
    public void Awake()
    {
        for(int i = 0; i < tempPatientList; i++) {
            tempNames[i] = "name " + i;
            tempDates[i] = "date " + i;
            tempIDs[i] = "ID " + i;
            tempProgress[i] = i*10;
        }
        Scene currentScene = SceneManager.GetActiveScene ();
        if (currentScene.name=="TherapistAccount"){
            Debug.Log("This is the TherapistAccount Scene");
            FillName(tempString/*Zaiyan's Return Therapist Name Function*/);
            FillDob(tempString/*Zaiyan's Return Therapist DOB Function*/);
            FillPhoneNum(tempString/*Zaiyan's Return Therapist PhoneNum Function*/);
            FillEmail(tempString/*Zaiyan's Return Therapist Email Function*/);
            FillAddress(tempString/*Zaiyan's Return Therapist Address Function*/);
        }
        if (currentScene.name=="TherapistAllPatients"){
            Debug.Log("This is the TherapistAllPatients Scene");
            FillAllPatients(tempNames, tempDates, tempIDs, tempProgress);
        }

        // Add listeners for buttons
        addPatientButton.onClick.AddListener(AddPatientButtonPressed);
        closeButton.onClick.AddListener(CloseAddPatientScreen);
    }

    // ==========Common Methods===========
    // Method to update Main Welcome Message
    public void setWelcomeMessage(String helloMessageReturn){
        helloMessage.text = helloMessageReturn;
    }
    // Methods For SideBar Buttons
    // Method for "My DashBoard" Button click
    public void TherapistDashboard(){
        SceneManager.LoadScene("TherapistDashboard");
    }

    // Method for "My Patients" Button click
    public void TherapistPatients(){
        SceneManager.LoadScene("TherapistAllPatients");
    }

    // Method for "My Account" Button click
    public void TherapistAccount(){
        SceneManager.LoadScene("TherapistAccount");
    }

    // Method for "Support" Button click
    public void TherapistSupport(){
        SceneManager.LoadScene("Support");
    }

    // Method for "Log Out" Button click
    public void TherapistLogOut(){
        SceneManager.LoadScene("SignIn");
    }

    // ==========TherapistAccount Scene Methods==========
    // Methods for Populating Therapist info

    public void FillName(string name){
        nameDataBox.text = "<cspace=-2>Dr.\n<cspace=-2>" + name;
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

    public void TherapistChangePassword(){
        SceneManager.LoadScene("ForgotPassword1");
    }

    // ==========TherapistAllPatients Scene Methods===========
    // =====Patient Box=====
    // Method to Populate Patient Names, Dates Joined, IDS, and Progress Bars
    public void FillAllPatients(string[] names, string[] dates, string[] IDs, float[] fills){
        if (SceneManager.GetActiveScene ().name == "TherapistAllPatients"){
            listLength = 10;
        }
        if (SceneManager.GetActiveScene ().name == "TherapistDashboard"){
            listLength = 5;

        }
        if(names.Length < listLength){
            for (int i = 0; i < names.Length; i++) {
                patientNames[i].text = names[i];
                patientJoinDates[i].text = dates[i];
                patientProgressBarFills[i].fillAmount = fills[i]/100;
                Debug.Log(fills[i]);
                patientIDs[i] = IDs[i];
            }
            for (int i = names.Length; i < listLength; i++) {
                patientNames[i].text = "";
                patientJoinDates[i].text = "";
                patientProgressBars[i].SetActive(false);
                patientContactButtons[i].SetActive(false);
            }
        }
        else{
            for (int i = 0; i < listLength; i++) {
                patientNames[i].text = names[i];
                patientJoinDates[i].text = dates[i];
                patientProgressBarFills[i].fillAmount = fills[i]/100;
                Debug.Log(fills[i]);
                patientIDs[i] = IDs[i];
            }
        }
        
    }

    // Method to View Single Patient from list
    public void ViewPatient (int index) {
        
    }
    // =====End of Patient Box=====

    // Method for Adding Patient Menu
    public void AddPatient(){
        addPatientScreen.SetActive(true);
    }
    public void CloseAddPatientScreen(){
        addPatientScreen.SetActive(false);
    }

    // Method to Confirm Adding Patient
    public void AddPatientButtonPressed()
    {
        string therapistId = RegisterController.TherapistId; // Use the actual therapist ID from RegisterController
        string firstNameText = firstName.text;
        string lastNameText = lastName.text;
        string emailText = email.text;
        string diagnosisText = diagnosis.text;

        TherapistController.AddPatient(therapistId, firstNameText, lastNameText, emailText, diagnosisText);
    }

    public void HandleAddPatientError(string errorMessage)
    {
        Debug.LogError("Error adding patient: " + errorMessage);
        // Display error message to the user
    }

    public void HandleAddPatientSuccess()
    {
        Debug.Log("Patient added successfully");
        CloseAddPatientScreen();
    }

    // Method to Search Patients
    public void SearchPatients(){
        string searParams = searchBar.text;
        Debug.Log("Search: " + searParams);
        // TherapistController.Search(searchParams);
        // then call the FillAllPatients
    }
}