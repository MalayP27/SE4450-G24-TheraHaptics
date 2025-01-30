using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginModel : MonoBehaviour
{
    private string email{ get; set; }
    private string password{ get; set; }
    private string serialKey{ get; set; }
    private string firstName{ get; set; }
    private string lastName{ get; set; }

    public LoginModel(){

    }
    public void SetEmail(string email){
        this.email= email;
    }
    public String GetEmail(){
        return this.email;
    }

    public void SetPassword(string password){
        this.password= password;
    }
    public String GetPassword(){
        return this.password;
    }
    
}

