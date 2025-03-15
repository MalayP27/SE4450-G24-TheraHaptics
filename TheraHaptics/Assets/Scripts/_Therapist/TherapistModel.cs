// Imports
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class Definition
public class TherapistModel : MonoBehaviour
{
    // Instantiating variables for the Therapist
    private string therapistID{ get; set; }
    private string firstName{ get; set; }
    private string lastName{ get; set; }
    private string email{ get; set; }
    private string productKeyID{ get; set; }
    private List<string> assignedPatients{ get; set; }

    // Constructor
    public TherapistModel(){

    }

    // Setter and Getter for therapistID
    public void SetTherapistID(string therapistID) {
        this.therapistID = therapistID;
    }
    public string GetTherapistID() {
        return this.therapistID;
    }

    // Setter and Getter for firstName
    public void SetFirstName(string firstName) {
        this.firstName = firstName;
    }
    public string GetFirstName() {
        return this.firstName;
    }

    // Setter and Getter for lastName
    public void SetLastName(string lastName) {
        this.lastName = lastName;
    }
    public string GetLastName() {
        return this.lastName;
    }

    // Setter and Getter for email
    public void SetEmail(string email) {
        this.email = email;
    }
    public string GetEmail() {
        return this.email;
    }

    // Setter and Getter for productKeyID
    public void SetProductKeyID(string productKeyID) {
        this.productKeyID = productKeyID;
    }
    public string GetProductKeyID() {
        return this.productKeyID;
    }

    // Setter and Getter for assignedPatients
    public void SetAssignedPatients(List<string> assignedPatients) {
        this.assignedPatients = assignedPatients;
    }
    public List<string> GetAssignedPatients() {
        return this.assignedPatients;
    }
}
