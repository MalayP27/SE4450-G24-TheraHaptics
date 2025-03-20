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
    static int tempListLength = 8;
    string[] tempNames = new string[tempListLength];
    string[] tempExercises = new string[tempListLength];
    string[] tempExerciseCompletions = new string[tempListLength];
    string[] tempDates = new string[tempListLength];
    string[] tempTypes = new string[tempListLength];
    string[] tempIDs = new string[tempListLength];
    float[] tempProgress = new float[tempListLength];

    // Permanent Variables
    //Number of exercise choices
    static int numExerciseChoices = 3;

    // List of exercise choices
    string[] exercises = new string[numExerciseChoices];
    
    // List of exercise IDs
    string[] exerciseIds = new string[numExerciseChoices];

    // List of exercises in plan, their time, and their reps and intensity

    int numOfCurrentExercises = 0;
    string[] exercisesInPlan = new string[5];
    float [] timeNums = new float[5];
    int[] repNums = new int[5];
    int intensity;

    // Number of displayed items
    int listLength;
    // Array for Holding which patients are in slots 1 to x
    private string[] patientIDs = new string[10];

    // Array for Holding which reports are in slots 1 to x
    private string[] reportIDs = new string[10];

    // Account info text boxes
    [Header("Account Boxes")]
    [SerializeField] private TMP_Text nameDataBox;
    [SerializeField] private TMP_Text genderDataBox;
    [SerializeField] private TMP_Text dobDataBox;
    [SerializeField] private TMP_Text phoneDataBox;
    [SerializeField] private TMP_Text emailDataBox;
    [SerializeField] private TMP_Text addressDataBox;
    
    // Patient text boxes
    [Header("All Patients Object lists")]
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

    [Header("All Reports Object lists")]
    [SerializeField] private TMP_Text[] reportPatientNames;
    [SerializeField] private TMP_Text[] reportDates;
    [SerializeField] private TMP_Text[] reportTypes;
    [SerializeField] private GameObject[] reportViewButtons;

    [Header("Create Exercise Plan Object lists")]
    [SerializeField] private TMP_Text[] exerciseChoices;
    [SerializeField] private TMP_Text[] planExercises;
    [SerializeField] private TMP_InputField[] times;
    [SerializeField] private TMP_InputField[] reps;
    [SerializeField] private TMP_InputField startDate;
    [SerializeField] private TMP_InputField endDate;
    [SerializeField] private TMP_InputField doctorsNotes;
    [SerializeField] private GameObject[] timeBoxes;
    [SerializeField] private GameObject[] repsBoxes;
    [SerializeField] private GameObject[] XButtons;
    [SerializeField] private Slider intensitySlider;

    [Header("Single Patient View Fields")]
    [SerializeField] private TMP_Text patientExercises;
    [SerializeField] private TMP_Text patientExerciseCompletions;
    [SerializeField] private Image[] exerciseProgressBarFills;
    [SerializeField] private Image totalProgressBarFill;
    [SerializeField] private TMP_Text planCompleted;
    [SerializeField] private TMP_Text activityLog;
    [SerializeField] private TMP_Text lastTime;
    [SerializeField] private TMP_Text exercisesCompleted;

    [Header("Misc")]
    [SerializeField] private TMP_Text helloMessage;
    [SerializeField] private TMP_InputField searchBar;

    // Accessing Error Message object and Component
    [SerializeField] private GameObject errorMessage;
    [SerializeField] private TMP_Text errorMessageText;

    [Header("Patient Dropdown")]
    [SerializeField] private TMP_Dropdown patientDropdown;

    [Header("Patient List")]
    [SerializeField] private Transform patientListContainer;
    [SerializeField] private GameObject patientPrefab;

    // List to store patient IDs
    private List<string> patientIds = new List<string>();

    // Constructor
    public TherapistView(){

    }

    // ==========Awake Method==========
    public void Awake()
    {
        // Defining exercises (rename these as you program the different gesture in)
     /*   exercises[0] = "exercise 1";
        exercises[1] = "exercise 2";
        exercises[2] = "exercise 3";
        exercises[3] = "exercise 4";
        exercises[4] = "exercise 5";
        exercises[5] = "exercise 6";
*/
        // Temp list tests
        for(int i = 0; i < tempListLength; i++) {
            tempNames[i] = "name " + i;
            tempExercises[i] = "exercise " + i;
            tempExerciseCompletions[i] = (i+1) + "/" + (i+1)*2;
            tempDates[i] = "date " + i;
            tempTypes[i] = "type " + i;
            tempIDs[i] = "ID " + i;
            tempProgress[i] = i*10;
        }

        // Find out which scene I'm on to awaken that scene
        Scene currentScene = SceneManager.GetActiveScene ();
        if (currentScene.name=="TherapistAccount"){
            Debug.Log("This is the TherapistAccount Scene");
            FillName(tempString, 1/*Zaiyan's Return Therapist Name Function*/);
            FillDob(tempString/*Zaiyan's Return Therapist DOB Function*/);
            FillPhoneNum(tempString/*Zaiyan's Return Therapist PhoneNum Function*/);
            FillEmail(tempString/*Zaiyan's Return Therapist Email Function*/);
            FillAddress(tempString/*Zaiyan's Return Therapist Address Function*/);
        }
        if (currentScene.name=="TherapistAllPatients"){
            Debug.Log("This is the TherapistAllPatients Scene");
            FillAllPatients(tempNames, tempDates, tempIDs, tempProgress);
        }
        if (currentScene.name=="TherapistAllReports"){
            Debug.Log("This is the TherapistAllReports Scene");
            FillAllReports(tempNames, tempDates, tempTypes, tempIDs);
        }
        if (currentScene.name=="TherapistCreateExercisePlan"){
            Debug.Log("This is the TherapistCreateExercisePlan Scene");
            for(int i = 0; i < exercises.Length; i++) {
                exerciseChoices[i].text = exercises[i];
            }
            for(int i = 0; i < timeBoxes.Length; i++) {
                planExercises[i].text = "";
                timeBoxes[i].SetActive(false);
                repsBoxes[i].SetActive(false);
                XButtons[i].SetActive(false);
            }
        }
        
        if (currentScene.name=="TherapistDashboard"){
            Debug.Log("This is the TherapistDashboard Scene");
       //     FillAllPatients(tempNames, tempDates, tempIDs, tempProgress);
            TherapistController.GetTherapist();
        }
        
        if (currentScene.name=="TherapistSinglePatient"){
            Debug.Log("This is the TherapistSinglePatient Scene");
            FillName(tempString, 0/*Zaiyan's Return Patient Name Function*/);
            FillGender(tempString/*Zaiyan's Return Patient Gender Function*/);
            FillDob(tempString/*Zaiyan's Return Patient DOB Function*/);
            FillPhoneNum(tempString/*Zaiyan's Return Patient PhoneNum Function*/);
            FillEmail(tempString/*Zaiyan's Return Patient Email Function*/);
            FillTimeTaken(tempString/*Zaiyan's Return Last time taken Function*/);
            FillExercisesCompleted(tempString/*Zaiyan's Return Exercises Completed Function*/);
            patientExercises.text = "";
            patientExerciseCompletions.text = "";
            for(int i = 0; i < 5; i++) {
                patientExercises.text += (i+1) + ". " + tempExercises[i] + "\n"; // Replace with actual data Zaiyan
                patientExerciseCompletions.text += tempExerciseCompletions[i] + "\n"; // Replace with actual data Zaiyan
                exerciseProgressBarFills[i].fillAmount = tempProgress[i]/100; // Replace with actual data Zaiyan
            }
            totalProgressBarFill.fillAmount = tempProgress[2]/100; // Replace with actual data Zaiyan
            planCompleted.text = tempProgress[2].ToString() + "% Plan Completed"; // Replace with actual data Zaiyan
            activityLog.text = tempString;
        }
    }

    private void Start()
    {
        // Start the coroutine to get the patient list
        StartCoroutine(TherapistController.GetPatientListDropdownCoroutine());
        StartCoroutine(TherapistController.GetPatientListDashboardCoroutine());
        StartCoroutine(TherapistController.GetAllExercisesCoroutine());
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

    // Methods for opening and closing Add Patient Menu
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

    // Method to Confirm Adding Patient
    public void HandleAddPatientError(string errorMessageTextReturn){
        errorMessageText.text = errorMessageTextReturn;
        errorMessage.SetActive(true);
    }

    // Method to Search Patients
    public void SearchPatients(){
        string searParams = searchBar.text;
        Debug.Log("Search: " + searParams);
        // TherapistController.Search(searchParams);
        // then call the FillAllPatients
    }

    // ==========TherapistAllReports Scene Methods==========
    // Method to Fill All Report Info
    public void FillAllReports(string[] patientNames, string[] dates, string[] types, string[] reportIDs){
        if (SceneManager.GetActiveScene ().name == "TherapistAllReports"){
            listLength = 10;
        }
        if(patientNames.Length < listLength){
            for (int i = 0; i < patientNames.Length; i++) {
                reportPatientNames[i].text = patientNames[i];
                reportDates[i].text = dates[i];
                reportTypes[i].text = types[i];
                reportIDs[i] = reportIDs[i];
            }
            for (int i = patientNames.Length; i < listLength; i++) {
                reportPatientNames[i].text = "";
                reportDates[i].text = "";
                reportTypes[i].text = "";
                reportViewButtons[i].SetActive(false);
            }
        }
        else{
            for (int i = 0; i < listLength; i++) {
                reportPatientNames[i].text = patientNames[i];
                reportDates[i].text = dates[i];
                reportTypes[i].text = types[i];
                reportIDs[i] = reportIDs[i];
            }
        }
    }

    // Method to Search Reports
    public void SearchReports(){
        string searParams = searchBar.text;
        Debug.Log("Search: " + searParams);
        // TherapistController.Search(searchParams);
        // then call the FillAllReports
    }

    // ==========TherapistCreateExercisePlan Scene Methods==========
    [Serializable]
    public class ExerciseListResponse
    {
        public Exercise[] exercises;
    }

    [Serializable]
    public class Exercise
    {
        public string exerciseId; // Add this field to store the exercise ID
        public string name;
        // You can add additional properties if needed (instructions, target_reps, etc.)
    }

        public void HandleGetAllExercisesSuccess(string responseBody)
    {
        Debug.Log("Exercises retrieved successfully: " + responseBody);

        // Wrap the JSON array in an object since JsonUtility doesn't support top-level arrays.
        string wrappedJson = "{\"exercises\":" + responseBody + "}";
        ExerciseListResponse exerciseList = JsonUtility.FromJson<ExerciseListResponse>(wrappedJson);
        
        // Populate the exercises and exerciseIds arrays with the exercise names and IDs.
        for (int i = 0; i < exerciseList.exercises.Length && i < exercises.Length; i++)
        {
            exercises[i] = exerciseList.exercises[i].name;
            exerciseIds[i] = exerciseList.exercises[i].exerciseId; // updated property name
            Debug.Log("Exercise ID: " + exerciseIds[i]);
        }

        // Optionally update the UI elements (e.g., the exercise choices text) with the new data.
        for (int i = 0; i < exercises.Length; i++)
        {
            exerciseChoices[i].text = exercises[i];
        }
    }


    public void LoadExercises(){
            StartCoroutine(TherapistController.GetAllExercisesCoroutine());
    }
    public void AddExercise(int indexOfExercise){
        if (numOfCurrentExercises < 5){
            planExercises[numOfCurrentExercises].text = exercises[indexOfExercise];
            timeBoxes[numOfCurrentExercises].SetActive(true);
            repsBoxes[numOfCurrentExercises].SetActive(true);
            XButtons[numOfCurrentExercises].SetActive(true);
            numOfCurrentExercises += 1;
        }
    }

    // Method to remove an exercise from the list
    public void RemoveExercise(int indexOfExercise){
        for(int i = indexOfExercise; i < numOfCurrentExercises-1; i++) {
            planExercises[i].text = planExercises[i+1].text;
            times[i].text = times[i+1].text;
            reps[i].text = reps[i+1].text;
        }
        numOfCurrentExercises -= 1;
        planExercises[numOfCurrentExercises].text = "";
        times[numOfCurrentExercises].text = "";
        reps[numOfCurrentExercises].text = "";
        timeBoxes[numOfCurrentExercises].SetActive(false);
        repsBoxes[numOfCurrentExercises].SetActive(false);
        XButtons[numOfCurrentExercises].SetActive(false);
    }

    // Method to submit Exercise Plan
    public void SubmitPlanButtonClicked()
    {
        // Assume 'numOfCurrentExercises' is the count of exercises currently added.
        int currentExercises = numOfCurrentExercises; // e.g., this.numOfCurrentExercises

        // Arrays to capture UI data for each exercise.
        string[] exercisesInPlanData = new string[5];
        float[] timeNumsData = new float[5];
        int[] repNumsData = new int[5];
        
        // Gather data from each added exercise.
        for (int i = 0; i < currentExercises; i++)
        {
            exercisesInPlanData[i] = planExercises[i].text;
            timeNumsData[i] = string.IsNullOrEmpty(times[i].text) ? 0 : float.Parse(times[i].text);
            repNumsData[i] = string.IsNullOrEmpty(reps[i].text) ? 0 : int.Parse(reps[i].text);
        }
        
        // Retrieve overall intensity from the slider.
        int overallIntensity = Convert.ToInt32(intensitySlider.value);
        string intensityText = "";
        if (overallIntensity == 1)
        {
            intensityText = "Easy";
        }
        else if (overallIntensity == 2)
        {
            intensityText = "Medium";
        }
        else if (overallIntensity == 3)
        {
            intensityText = "Hard";
        }
        else if (overallIntensity == 4)
        {
            intensityText = "Intense";
        }
        
        // Get the start and end dates from the input fields.
        string startDateStr = startDate.text;
        string endDateStr = endDate.text;
        
        // Retrieve doctor's notes from the input field.
        string doctorsNotesText = doctorsNotes.text;
        
        // Retrieve the selected patient ID.
        string selectedPatientId = patientIds[patientDropdown.value];
        
        Debug.Log("Submitting Exercise Program with the following details:");
        Debug.Log("Exercises: " + string.Join(", ", exercisesInPlanData));
        Debug.Log("Times: " + string.Join(", ", timeNumsData));
        Debug.Log("Reps: " + string.Join(", ", repNumsData));
        Debug.Log("Start Date: " + startDateStr + ", End Date: " + endDateStr);
        Debug.Log("Intensity: " + intensityText);
        Debug.Log("Doctor's Notes: " + doctorsNotesText);
        Debug.Log("Patient ID: " + selectedPatientId);
        
        // Call the submission method.
        TherapistController.SubmitExercisePlan(currentExercises, exercisesInPlanData, timeNumsData, repNumsData, startDateStr, endDateStr, intensityText, doctorsNotesText, selectedPatientId, exerciseIds);
    }

    // Method to show error message when exercise plan is created wrong
    public void SubmitPlanFailure(string errorMessageTextReturn){
        errorMessageText.text = errorMessageTextReturn;
        errorMessage.SetActive(true);    
    }

    // Method to handle successful exercise plan creation
    public void SubmitPlanSuccess(){
        TherapistDashboard();
    }

    // ==========TherapistCreateExercisePlan Scene Methods==========
    // Method go to create exercise program scene
    public void CreateExerciseProgram(){
        SceneManager.LoadScene("TherapistCreateExercisePlan");
    }

    // Method go to reports scene
    public void ViewAllReports(){
        SceneManager.LoadScene("TherapistAllReports");
    }

    // ==========TherapistSinglePatient Scene Methods==========
    public void FillTimeTaken(string time){
        lastTime.text = "Time Taken\n<size=14>" + time;
    }
    public void FillExercisesCompleted(string exerComp){
        exercisesCompleted.text = "Exercises Completed\n<size=14>" + exerComp;
    }
    // Method to Confirm Adding Patient
    public void HandleAddPatientSuccess()
    {
        Debug.Log("Patient added successfully");
        CloseAddPatientScreen();
    }

    public void HandleGetPatientListDropdownSuccess(string responseBody)
    {
        Debug.Log("Patient list retrieved successfully: " + responseBody);

        // Wrap the JSON array in an object
        string wrappedJson = "{\"patients\":" + responseBody + "}";

        // Parse the response to extract patient names
        List<string> patientNames = new List<string>();
        PatientListWrapper patientList = JsonUtility.FromJson<PatientListWrapper>(wrappedJson);

        // Clear the existing patient IDs list
        patientIds.Clear();
        foreach (var patient in patientList.patients)
        {
            Debug.Log("Patient: " + patient.firstName + " " + patient.lastName);
            patientDropdown.gameObject.SetActive(true);
            patientNames.Add(patient.firstName + " " + patient.lastName);
            patientIds.Add(patient.patientId); // Store the patient ID

        }

        // Populate the dropdown list
        patientDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (var name in patientNames)
        {
            options.Add(new TMP_Dropdown.OptionData(name));
        }
        patientDropdown.AddOptions(options);
        Debug.Log("Dropdown options updated.");
    }

    public void HandleGetPatientListDashboardSuccess(string responseBody)
    {
        Debug.Log("DASHBOARD Raw responseBody:\n" + responseBody);

        // Wrap the JSON
        string wrappedJson = "{\"patients\":" + responseBody + "}";
        Debug.Log("DASHBOARD Wrapped JSON:\n" + wrappedJson);

        // Deserialize
        PatientListResponse patientList = JsonUtility.FromJson<PatientListResponse>(wrappedJson);

        // Log how many patients we got
        Debug.Log("DASHBOARD # of patients from server: " + patientList.patients.Length);

        // Clear existing
        foreach (Transform child in patientListContainer)
        {
            Destroy(child.gameObject);
        }

        // Iterate through each patient
        foreach (var patient in patientList.patients)
        {
            Debug.Log($"DASHBOARD Loop: {patient.firstName} {patient.lastName} (ID: {patient.patientId})");

            GameObject patientItem = Instantiate(patientPrefab, patientListContainer);
            TMP_Text nameText = patientItem.transform.Find("Name").GetComponent<TMP_Text>();
            nameText.text = patient.firstName + " " + patient.lastName;

 //          Image progressBar = patientItem.transform.Find("ProgressBar").GetComponent<Image>();
 //           progressBar.fillAmount = 0.5f; // or your real data

            Button contactButton = patientItem.transform.Find("ContactButton").GetComponent<Button>();
            contactButton.onClick.AddListener(() => ContactPatient(patient.patientId));
        }
    }


    public void HandleGetPatientListError(string errorMessage)
    {
        Debug.LogError(errorMessage);
    }
   
    private void ContactPatient(string patientId)
    {
        Debug.Log("Contacting patient with ID: " + patientId);
        // Implement contact logic here
    }
    
    [Serializable]
    public class PatientListResponse
    {
        public Patient[] patients;

    }

    [Serializable]
    public class Patient
    {
        public string patientId;
        public string firstName;
        public string lastName;
    //    public DateTime dateJoined;
    }

    [Serializable]
    public class PatientListWrapper
    {
        public Patient[] patients;
    }
}