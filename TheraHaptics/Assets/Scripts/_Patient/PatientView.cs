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
    static int tempListLength = 8;
    string[] tempExercises = new string[tempListLength];
    string[] tempExerciseCompletions = new string[tempListLength];
    float[] tempProgress = new float[tempListLength];
    string[] tempDates = new string[tempListLength];
    string[] tempTypes = new string[tempListLength];
    string[] tempIDs = new string[tempListLength];

    // Permanent Variables
    // Current Exercise Time in seconds
    private float time = 0;
    // Current exercise time formatted to HH:MM:SS
    private string timeText = "";
    // Current Reps
    private int reps = 0;
    // Number of displayed items
    int listLength;
    
    // Account info text boxes
    [Header("Account Boxes")]
    [SerializeField] private TMP_Text nameDataBox;
    [SerializeField] private TMP_Text genderDataBox;
    [SerializeField] private TMP_Text dobDataBox;
    [SerializeField] private TMP_Text phoneDataBox;
    [SerializeField] private TMP_Text emailDataBox;
    [SerializeField] private TMP_Text addressDataBox;

    [Header("Dashboard Info Boxes")]
    [SerializeField] private TMP_Text patientExercises;
    [SerializeField] private TMP_Text patientExerciseCompletions;
    [SerializeField] private Image[] exerciseProgressBarFills;
    [SerializeField] private Image totalProgressBarFill;
    [SerializeField] private TMP_Text planCompleted;
    [SerializeField] private TMP_Text goals;
    [SerializeField] private TMP_Text startDateBox;
    [SerializeField] private TMP_Text endDateBox;
    [SerializeField] private TMP_Text sessionsCompletedBox;
    [SerializeField] private TMP_Text lastTime;
    [SerializeField] private TMP_Text exercisesCompleted;

    [Header("Start Session Boxes")]
    [SerializeField] private TMP_Text exerciseProgramName;
    [SerializeField] private TMP_Text timeEst;
    [SerializeField] private TMP_Text intensityEst;
    [SerializeField] private TMP_Text programGoals;
    [SerializeField] private TMP_Text startDate;
    [SerializeField] private TMP_Text endDate;

    [Header("Current Exercise Boxes")]
    [SerializeField] private TMP_Text currentExerciseName;
    [SerializeField] private TMP_Text currentInstructions;
    [SerializeField] private TMP_Text currentTargetTime;
    [SerializeField] private TMP_Text currentTargetReps;
    [SerializeField] private TMP_Text currentTime;
    [SerializeField] private TMP_Text currentReps;
    [SerializeField] private Image currentExerciseExample;
    [SerializeField] private Sprite[] exerciseImages;
    [SerializeField] private TMP_Text pauseButton;
    [SerializeField] private GameObject sessionIsPausedText;

    [Header("PainReportingDetails")]
    //[SerializeField] private ClickMover painLocation;
    [SerializeField] public TMP_InputField painDescription;
    [SerializeField] public Slider painIntensitySlider;

    [Header("Session Finish Details")]
    [SerializeField] private TMP_InputField sessionFeedback;

    [Header("Patient Reports boxes")]
    [SerializeField] private TMP_Text[] reportDates;
    [SerializeField] private TMP_Text[] reportTypes;
    [SerializeField] private GameObject[] reportViewButtons;
    [SerializeField] private TMP_InputField searchBar;

    [Header("Misc")]
    // Temporary Sprite for exercise Screen
    [SerializeField] private Sprite tempExerciseImage;
    [SerializeField] private Sprite thumbsUpExerciseImage;
    [SerializeField] private Sprite fistExerciseImage;
    // Text Boxes
    [SerializeField] private TMP_Text helloMessage;

    // Constructor
    public PatientView(){

    }

    // ==========Awake Method==========
    public void Awake()
    {
        // Temp list tests
        for(int i = 0; i < tempListLength; i++) {
            tempExercises[i] = "exercise " + i;
            tempExerciseCompletions[i] = (i+1) + "/" + (i+1)*2;
            tempProgress[i] = i*10;
            tempDates[i] = "date " + i;
            tempTypes[i] = "type " + i;
            tempIDs[i] = "ID " + i;
        }

        // Find out which scene I'm on to awaken that scene
        Scene currentScene = SceneManager.GetActiveScene ();
        if (currentScene.name=="PatientAccount"){
            Debug.Log("This is the PatientAccount Scene");
            FillName(tempString, tempString/*Zaiyan's Return Patient Name Function*/);
            FillGender(tempString/*Zaiyan's Return Patient Name Function*/);
            FillDob(tempString/*Zaiyan's Return Patient DOB Function*/);
            FillPhoneNum(tempString/*Zaiyan's Return Patient PhoneNum Function*/);
            FillEmail(tempString/*Zaiyan's Return Patient Email Function*/);
            FillAddress(tempString/*Zaiyan's Return Patient Address Function*/);
        }
        if (currentScene.name=="PatientDashboard"){
            Debug.Log("This is the PatientDashboard Scene");
            // Start the coroutine to fetch and display the patient data instead of using the placeholder
            StartCoroutine(PatientController.GetPatientCoroutine());
        //  setWelcomeMessage(tempString);
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
            goals.text = tempString;
            startDateBox.text = "Start Date: " + tempString; // Replace with actual data Zaiyan
            endDateBox.text = "End Date: " + tempString; // Replace with actual data Zaiyan
            sessionsCompletedBox.text = "Sessions Completed: " + tempString; // Replace with actual data Zaiyan
        }
        if (currentScene.name=="PatientStartSession"){
            PatientController.GetExerciseProgram();
            // patientExercises.text = "";
            // for(int i = 0; i < 5; i++) {
            //     patientExercises.text += (i+1) + ". " + tempExercises[i] + "\n"; // Replace with actual data Zaiyan
            // }
            // exerciseProgramName.text = tempString;
            // timeEst.text = "<B>Estimated Time: </B>" + tempString + " minutes"; // Replace with actual data Zaiyan
            // intensityEst.text = "<B>Intensity Level: </B>" + tempString; // Replace with actual data Zaiyan
            
        }
        if (currentScene.name=="PatientExercise"){
            PatientController.GetExerciseProgram();
            currentReps.text = "Repetitions: " + reps;
            
            // currentExerciseName.text = tempString; // Replace with actual data Zaiyan
            // timeEst.text = "<B>Estimated Time: </B>" + tempString + " minutes"; // Replace with actual data Zaiyan
            // intensityEst.text = "<B>Intensity Level: </B>" + tempString; // Replace with actual data Zaiyan
            // currentInstructions.text = "<B>Instructions: </B>" + tempString; // Replace with actual data Zaiyan
            // currentTargetTime.text = "Duration: " + tempString; // Replace with actual data Zaiyan
            // currentTargetReps.text = "Repetitions: " + tempString;
              // Replace with actual data Zaiyan
            //currentExerciseExample.sprite = tempExerciseImage; // Replace with exerciseImages[TherapistController.ReturnIndexOfExerciseImage()] 
            
        }
        if (currentScene.name=="PatientEndSession"){
            FillTimeTaken(tempString/*Zaiyan's Return Last time taken Function*/);
            FillExercisesCompleted(tempString/*Zaiyan's Return Exercises Completed Function*/);
            patientExercises.text = "";
            patientExerciseCompletions.text = "";
            for(int i = 0; i < 5; i++) {
                patientExercises.text += (i+1) + ". " + tempExercises[i] + "\n"; // Replace with actual data Zaiyan
                patientExerciseCompletions.text += tempExerciseCompletions[i] + "\n"; // Replace with actual data Zaiyan
                exerciseProgressBarFills[i].fillAmount = tempProgress[i]/100; // Replace with actual data Zaiyan
            }
        }
        if (currentScene.name=="PatientReports"){
            FillAllReports(tempDates, tempTypes, tempIDs);
        }
    }
    public void Update(){
        Scene currentScene = SceneManager.GetActiveScene ();
        if (currentScene.name=="PatientExercise"){
            time += Time.deltaTime;
            int hours = Mathf.FloorToInt(time / 3600);
            int minutes = Mathf.FloorToInt((time % 3600) / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);;
            currentTime.text = "Duration: " + timeText;
        }
    }
        
    // ==========Common Methods===========
    // Method to update Main Welcome Message
    public void setWelcomeMessage(String helloMessageReturn){
        helloMessage.text = "Hello " + helloMessageReturn + ", Welcome";
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

    // ==========PatientAccount Scene Methods==========
    // Methods for Populating info boxes
    public void FillName(string fName, string lName){
            nameDataBox.text = fName + "\n" + lName;
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

    // ==========PatientDashboard Scene Methods==========
    // Method to fill the last time taken box
    public void FillTimeTaken(string time){
        lastTime.text = "Time Taken\n<size=14>" + time;
        if (SceneManager.GetActiveScene().name == "PatientEndSession"){
            lastTime.text = "Time Taken\n<size=30>" + time;
        }
    }

    // Method to fill the last exercises completed box
    public void FillExercisesCompleted(string exerComp){
        exercisesCompleted.text = "Exercises Completed\n<size=14>" + exerComp;
        if (SceneManager.GetActiveScene().name == "PatientEndSession"){
            exercisesCompleted.text = "Exercises Completed\n<size=30>" + exerComp;
        }
    }

    // Method for when the start session button is pressed
    public void PatientStartSession(){
        SceneManager.LoadScene("PatientStartSession");
    }

    // Method for when the report pain button is pressed
    public void PatientReportPain(){
        if (SceneManager.GetActiveScene().name == "PatientDashboard"){
            SceneManager.LoadScene("PatientPainReporting");
        }
        else{
            SaveAndCompleteSession();
            SceneManager.LoadScene("PatientPainReporting");
        }
    }
    
    // ==========PatientStartSession Scene Methods==========
    // Method to notify the controller to save the details for the next exercise to load and then load the scene where the awake method will load the saved details according to which exercise is next
    public void PatientLoadStartExercise(int nextOrPrev){
        // TherapistController.SaveCurrentAndGoNextExercise(int nextOrPrev, string timerText, int reps) (nextOrPrev is 0 for previous exercise, 1 for next, if they click previous on the first, either reload same one, or go to dashboard, if next on last, call the EndSession Function)
        SceneManager.LoadScene("PatientExercise");
    }
    
    public void PatientLoadNextExercise(int nextOrPrev){
        // TherapistController.SaveCurrentAndGoNextExercise(int nextOrPrev, string timerText, int reps) (nextOrPrev is 0 for previous exercise, 1 for next, if they click previous on the first, either reload same one, or go to dashboard, if next on last, call the EndSession Function)
        PatientController.currentExerciseIndex ++;
        SceneManager.LoadScene("PatientExercise");
    }

    public void SetExerciseProgram(PatientController.ExerciseProgram exerciseProgram)
    {
        // Check if the exerciseProgram is null
        if (exerciseProgram == null)
        {
            Debug.LogError("ExerciseProgram is null!");
            return;
        }
        // Check if the exercises list is null
        if (exerciseProgram.exercises == null)
        {
            Debug.LogError("ExerciseProgram.exercises is null!");
            return;
        }

        Debug.Log("Setting exercise program: " + exerciseProgram.name);

        // Update UI elements with exercise program details
        exerciseProgramName.text = exerciseProgram.name;
        timeEst.text = "<B>Estimated Time: </B>" + exerciseProgram.estimatedTime + " minutes";
        intensityEst.text = "<B>Intensity Level: </B>" + exerciseProgram.intensity;
        programGoals.text = "<B>Program Goals: </B>" + exerciseProgram.planGoals;
        startDate.text = "Start Date: " + exerciseProgram.startDate;
        endDate.text = "End Date: " + exerciseProgram.endDate;

        // Clear existing exercise texts
        patientExercises.text = "";
        // patientExerciseCompletions.text = "";

        // Loop through each exercise in the program
        for (int i = 0; i < exerciseProgram.exercises.Count; i++)
        {
            Debug.Log("gptpls");
            PatientController.Exercise exercise = exerciseProgram.exercises[i];

            if (exercise == null)
            {
                Debug.LogWarning("Exercise at index " + i + " is null!");
                continue;
            }

            patientExercises.text += (i + 1) + ". " + exercise.name + "\n";
    //      patientExerciseCompletions.text += exercise.targetReps + " reps, " + exercise.targetDuration + " seconds\n";
        }
        Debug.Log("Final patientExercises.text:\n" + patientExercises.text);
    }

    // ==========PatientExercise Scene Methods==========
    // Method to add a repetition to the screen (call whenever the haptic glove detects a rep of the specified exercise)
    
    public void SetCurrentExercise(PatientController.Exercise exercise)
    {
        if (exercise == null)
        {
            Debug.LogError("Exercise is null!");
            return;
        }

        currentExerciseName.text = exercise.name;
        currentInstructions.text = "<B>Instructions: </B>" + exercise.instructions;
        currentTargetTime.text = "Duration: " + exercise.targetDuration + " seconds";
        currentTargetReps.text = "Repetitions: " + exercise.targetReps;
        timeEst.text = "<B>Estimated Time: </B>" + exercise.targetDuration + " minutes";
        intensityEst.text = "<B>Intensity Level: </B>" + exercise.intensity;
        
        // Determine which sprite to display based on the exercise name
        if (exercise.name == "Thumbs Up")
        {
            currentExerciseExample.sprite = thumbsUpExerciseImage;
        }
        else if (exercise.name == "Fist")
        {
            currentExerciseExample.sprite = fistExerciseImage;
        }
        else
        {
            Debug.LogWarning("No matching sprite found for exercise: " + exercise.name);
            currentExerciseExample.sprite = tempExerciseImage; // Fallback to a default image
        }
    }
    public void AddRepetition (){
        reps += 1;
        currentReps.text = "Repetitions: " + reps;
    }

    // Method for pausing and unpausing the session
    public void PauseAndUnpauseSession (){
        if (Time.timeScale == 0){
            // TherapistController.HandlePause(0); (this method should handle the stopping of calling the AddRepetition() Method when it receives a 1 and resume the calling of that method when it receives a 0 to avoid adding reps when the session is paused)
            Time.timeScale = 1;
            pauseButton.text = "<cspace=-2>Pause Session";
            sessionIsPausedText.SetActive(false);
        }
        else{
            // TherapistController.HandlePause(1); (this method should handle the stopping of calling the AddRepetition() Method when it receives a 1 and resume the calling of that method when it receives a 0 to avoid adding reps when the session is paused)
            Time.timeScale = 0;
            pauseButton.text = "<cspace=-2>Resume Session";
            sessionIsPausedText.SetActive(true);
        }
    }
    // Method to end the session
    public void EndSession(){
        // TherapistController.SaveLastExercise(string timerText,int reps)
        PatientController.currentExerciseIndex = 0;
        SceneManager.LoadScene("PatientEndSession");
    }
    
    // ==========PatientPainReporting Scene Methods==========
    // Method for when the Pain Report is submitted
    public void SendPainReport()
    {
        // Retrieve pain report details from the UI.
        int painIntensity = Convert.ToInt32(painIntensitySlider.value);
        string painDescriptionText = painDescription.text;

        // Optional: Validate input (e.g., ensure description is not empty and intensity is valid)
        if (string.IsNullOrEmpty(painDescriptionText) || painIntensity <= 0)
        {
            Debug.LogError("Pain description is empty or pain intensity is not valid.");
            return;
        }

        // Find the PatientController instance and start the pain report coroutine.
        PatientController patientController = FindObjectOfType<PatientController>();
        if (patientController != null)
        {
            StartCoroutine(patientController.SendPainReportCoroutine());
        }
        else
        {
            Debug.LogError("PatientController instance not found!");
        }
    }

    // ==========PatientEndSession Scene Methods==========
    // Method to save and complete the current session
    public void SaveAndCompleteSession(){
        if (SceneManager.GetActiveScene().name == "PatientEndSession"){
            PatientController.currentExerciseIndex = 0;
            string feedback = sessionFeedback.text;
            // TherapistController.SaveFeedbackToSession(feedback) and then call PatientDashboard
        }
        else{
            // TherapistController.SaveFeedbackToSession("Session Ended Early") and then call PatientDashboard
        }
    }

    // Method to restart the session
    public void PatientRestartSession(){
        PatientController.currentExerciseIndex = 0;
        SaveAndCompleteSession();
        PatientStartSession();
    }

    // ==========PatientReports Scene Methods==========
    // Method to Fill All Report Info
    public void FillAllReports(string[] dates, string[] types, string[] reportIDs){
        if (SceneManager.GetActiveScene().name == "PatientReports"){
            listLength = 10;
        }
        if(dates.Length < listLength){
            for (int i = 0; i < dates.Length; i++) {
                reportDates[i].text = dates[i];
                reportTypes[i].text = types[i];
                reportIDs[i] = reportIDs[i];
            }
            for (int i = dates.Length; i < listLength; i++) {
                reportDates[i].text = "";
                reportTypes[i].text = "";
                reportViewButtons[i].SetActive(false);
            }
        }
        else{
            for (int i = 0; i < listLength; i++) {
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
}
