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
    // Text Boxes
    [SerializeField] private TMP_Text helloMessage;

    // Constructor
    public PatientView(){

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
}
