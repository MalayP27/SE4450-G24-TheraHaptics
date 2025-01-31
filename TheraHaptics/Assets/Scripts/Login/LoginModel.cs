// Imports
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class Definition
public class LoginModel : MonoBehaviour
{
    // Instantiating variables for the Login
    private string email{ get; set; }
    private string password{ get; set; }
    private string productKey{ get; set; }
    private string firstName{ get; set; }
    private string lastName{ get; set; }

    // Constructor
    public LoginModel(){

    }

    // Setter and Getter for email
    public void SetEmail(string email){
        this.email= email;
    }
    public String GetEmail(){
        return this.email;
    }

    // Setter and Getter for password
    public void SetPassword(string password){
        this.password= password;
    }
    public String GetPassword(){
        return this.password;
    }

    // Setter and Getter for product key
    public void SetProductKey(string productKey){
        this.productKey = productKey;
    }
    public String GetSProductKey(){
        return this.productKey;
    }

    // Setter and Getter for first name
    public void SetFirstName(string firstName){
        this.firstName= firstName;
    }
    public String GetFirstName(){
        return this.firstName;
    }
    
    // Setter and Getter for last name
    public void SetLastName(string lastName){
        this.lastName= lastName;
    }
    public String GetLastName(){
        return this.lastName;
    }

}