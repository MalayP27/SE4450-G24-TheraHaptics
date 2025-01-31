using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginController : MonoBehaviour
{

    public static LoginModel loginModel = new LoginModel();
    [SerializeField] public static LoginView loginView = new LoginView();
    // Start is called before the first frame update
    private void Awake()
    {
        loginView = GetComponent<LoginView>();
    }

    public static void SignIn(string email, string password){
        // INPUT VALIDATION (use HandleError accordingly)
        // API access and call result use the functions below for failed and successful login respectively
        Debug.Log(email + ", " + password);
        //loginView.HandleError("ReturnedErrortext");
        //loginView.HandleSignInSuccess(bool userType(false for therapist, true for patient));
        
    }
}
