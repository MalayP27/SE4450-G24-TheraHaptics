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
    public void Dashboard(){
        SceneManager.LoadScene(1);
    }

    // Called by clicking My Account; goes to account scene
    public void Account(){
        SceneManager.LoadScene(2);
    }

    // Called by clicking My Patients; goes to patients scene
    public void Patients(){
        SceneManager.LoadScene(3);
    }

    // Called when Clicking Support; goes to support scene
    public void Support(){
        SceneManager.LoadScene(5);
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
        SceneManager.LoadScene(4);
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

}
