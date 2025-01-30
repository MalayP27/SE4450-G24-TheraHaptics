using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    [SerializeField] public static LoginView loginView = new LoginView();
    // Start is called before the first frame update
    private void Awake()
    {
        loginView = GetComponent<LoginView>();
    }

    public static void SignIn(LoginModel model){
        // API access and call result use the functions below for failed and successful login respectively
        Debug.Log(model.GetEmail());
        //loginView.HandleError("ReturnedErrortext");
        //loginView.HandleSignInSuccess(bool userType(false for therapist, true for patient));
        
    }
}
