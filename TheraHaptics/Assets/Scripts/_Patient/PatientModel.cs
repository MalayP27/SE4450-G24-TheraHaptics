// Imports
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class Definition
public class PatientModel : MonoBehaviour
{
    // Instantiating variables for the Patient
    private string patientID{ get; set; }
    private string firstName{ get; set; }
    private string lastName{ get; set; }
    private string email{ get; set; }
    private string phoneNumber{ get; set; }
    private string address{ get; set; }
    private string DOB{ get; set; }
    private string gender{ get; set; }
    private string diagnosis{ get; set; }
    private string dateJoined{ get; set; }
    private string goalID{ get; set; }
    private string therapistID{ get; set; }

    // Constructor
    public PatientModel(){

    }

    // Setter and Getter for patientID
    public void SetPatientID(string patientID) {
        this.patientID = patientID;
    }
    public string GetPatientID() {
        return this.patientID;
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

    // Setter and Getter for phoneNumber
    public void SetPhoneNumber(string phoneNumber) {
        this.phoneNumber = phoneNumber;
    }
    public string GetPhoneNumber() {
        return this.phoneNumber;
    }

    // Setter and Getter for address
    public void SetAddress(string address) {
        this.address = address;
    }
    public string GetAddress() {
        return this.address;
    }

    // Setter and Getter for DOB
    public void SetDOB(string DOB) {
        this.DOB = DOB;
    }
    public string GetDOB() {
        return this.DOB;
    }

    // Setter and Getter for gender
    public void SetGender(string gender) {
        this.gender = gender;
    }
    public string GetGender() {
        return this.gender;
    }

    // Setter and Getter for diagnosis
    public void SetDiagnosis(string diagnosis) {
        this.diagnosis = diagnosis;
    }
    public string GetDiagnosis() {
        return this.diagnosis;
    }

    // Setter and Getter for dateJoined
    public void SetDateJoined(string dateJoined) {
        this.dateJoined = dateJoined;
    }
    public string GetDateJoined() {
        return this.dateJoined;
    }

    // Setter and Getter for goalID
    public void SetGoalID(string goalID) {
        this.goalID = goalID;
    }
    public string GetGoalID() {
        return this.goalID;
    }

    // Setter and Getter for therapistID
    public void SetTherapistID(string therapistID) {
        this.therapistID = therapistID;
    }
    public string GetTherapistID() {
        return this.therapistID;
    }
}
