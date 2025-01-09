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

    // Called by clicking My Dashboard; goes to dashboard scene
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
        SceneManager.LoadScene(4);
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
        
    }

    public void OpenPatientChat(){
        
    }

    public void ViewPatientMore(){
        
    }

    public void AddPatient(){
        
    }

}
