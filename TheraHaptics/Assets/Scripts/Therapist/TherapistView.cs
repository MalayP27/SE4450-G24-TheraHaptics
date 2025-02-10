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
}
