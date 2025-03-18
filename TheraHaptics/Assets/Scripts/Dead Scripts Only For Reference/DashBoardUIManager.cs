using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DashBoardUIManager : MonoBehaviour
{
    // Getting input fields
    [SerializeField] private TMP_InputField searchBox;


    // Getting PopUp Box
    [SerializeField] private GameObject personalInfoScreen;
    // Called by clicking My Dashboard; goes to dashboard scene

    // Makes sure the PopUp isn't immediately visible
    private void Awake(){
        personalInfoScreen.SetActive(false);
    }
    public void PHDashboard(){
        SceneManager.LoadScene(5);
    }

    // Called by clicking My Account; goes to account scene
    public void PHAccount(){
        SceneManager.LoadScene(6);
    }

    // Called by clicking My Patients; goes to patients scene
    public void PHPatients(){
        SceneManager.LoadScene(7);
    }

    public void PDashboard(){
        SceneManager.LoadScene(9);
    }

    // Called by clicking My Patients; goes to patients scene
    public void PReports(){
        SceneManager.LoadScene(10);
    }

    // Called by clicking My Account; goes to account scene
    public void PAccount(){
        SceneManager.LoadScene(11);
    }

    // Called when Clicking Support; goes to support scene
    public void Support(){
        SceneManager.LoadScene(13);
    }

    // Logs the user out
    public void LogOut(){
        SceneManager.LoadScene(0);
    }

    public void PatientSearch(){
        
    }

    public void ViewAll(){
        
    }
    
    public void ViewPatient(){
        // Check which patient here and save to staticdata
        SceneManager.LoadScene(8);
    }

    public void OpenPatientChat(){
        
    }

    public void ViewPatientMore(){
        
    }

    public void AddPatient(){
        
    }
    public void ShowPersonalInfo(){
        personalInfoScreen.SetActive(true);
    }

    public void HidePersonalInfo(){
        personalInfoScreen.SetActive(false);
    }

    public void ShowChart(){
        
    }

    public void ReportPain(){
        
    }

    public void ViewReport(){
        
    }

    public void StartSession(){
        SceneManager.LoadScene(12);
    }
}
