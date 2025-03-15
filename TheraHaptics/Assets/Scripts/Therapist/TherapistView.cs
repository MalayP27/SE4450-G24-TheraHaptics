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
    // Text Boxes
    [SerializeField] private TMP_Text helloMessage;
    [SerializeField] private TMP_Text[] patientNames;
    [SerializeField] private TMP_Text[] patientJoinDates;

    // Array for Holding which patients are in slots 1 to 5
    private string[] patientIDs;

    // Constructor
    public TherapistView(){

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
        SceneManager.LoadScene("TherapistPatients");
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

    // ==========TherapistPatients Scene Methods===========
    // =====Patient Box=====
    // Method to Populate Patient Names, Dates Joined, and IDS
    public void FillPatients(string[] names, string[] dates, string[] IDs){
        for (int i = 0; i < 5; i++) {
            patientNames[i].text = names[i];
            patientJoinDates[i].text = dates[i];
            patientIDs[i] = IDs[i];
        }
    }

    // Method to View Single Patient from list
    public void ViewPatient (int index) {
        
    }

    // Method to Chat with Single Patient from list
    public void ChatPatient (int index) {
        
    }

    // Method to Show Patient More from list
    public void MorePatient (int index) {
        
    }
    // Method to Add Patient
    public void AddPatient(){

    }
    // =====End of Patient Box=====

    // Method to View All Patients
    public void ViewAllPatients(){

    }
    // Method to Search Patients
    public void SearchPatients(){

    }
}
