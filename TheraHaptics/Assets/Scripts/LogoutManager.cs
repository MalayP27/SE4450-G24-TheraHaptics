using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoutManager : MonoBehaviour
{
    public Button logoutButton; // Reference to the Logout button

    void Start()
    {
        // Set up button listener
        logoutButton.onClick.AddListener(OnLogout);
    }

    void OnLogout()
    {
        // Clear any session data if needed (e.g., cookies, tokens)
        Debug.Log("Logging out...");

        // Load Scene 0 (e.g., Login scene)
        SceneManager.LoadScene(0);
    }
}