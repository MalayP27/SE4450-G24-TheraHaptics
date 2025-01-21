using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;

public class PatientManager : MonoBehaviour
{
    public GameObject addPatientModal; // Reference to AddPatientModal panel
    public Button addPatientButton; // Reference to Add Patient button
    public TMP_InputField firstNameInput; // TMP_InputField for First Name
    public TMP_InputField lastNameInput; // TMP_InputField for Last Name
    public TMP_InputField emailInput; // TMP_InputField for Email
    public TMP_InputField diagnosisInput; // TMP_InputField for Diagnosis
    public Button submitButton;

    private const string ApiUrl = "http://localhost:5089/api/therapist/newPatient";

    void Start()
    {
        // Set up the button listeners
        addPatientButton.onClick.AddListener(ToggleAddPatientModal);
        submitButton.onClick.AddListener(OnSubmitPatient);
    }

    void ToggleAddPatientModal()
    {
        // Toggle modal visibility
        addPatientModal.SetActive(!addPatientModal.activeSelf);
    }

    void OnSubmitPatient()
    {
        // Gather input field values
        string firstName = firstNameInput.text;
        string lastName = lastNameInput.text;
        string email = emailInput.text;
        string diagnosis = diagnosisInput.text;

        // Validate fields
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
            string.IsNullOrEmpty(email) || string.IsNullOrEmpty(diagnosis))
        {
            Debug.LogError("All fields must be filled!");
            return;
        }

        // Create patient data
        PatientPostDto newPatient = new PatientPostDto
        {
            firstName = firstName,
            lastName = lastName,
            emailAddress = email,
            diagnosis = diagnosis,
            therapistId = "677eb2e8d55cc2e7a7bb29b3" // Replace with actual therapist ID
        };

        // Send the POST request
        StartCoroutine(PostNewPatient(newPatient));
    }

    IEnumerator PostNewPatient(PatientPostDto patient)
    {
        string jsonData = JsonUtility.ToJson(patient);
        using (UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Patient added successfully!");
                ToggleAddPatientModal(); // Close the modal
            }
            else
            {
                Debug.LogError($"Error adding patient: {request.error}");
            }
        }
    }
}

// Define the DTO class for sending patient data
[System.Serializable]
public class PatientPostDto
{
    public string firstName;
    public string lastName;
    public string emailAddress;
    public string diagnosis;
    public string therapistId;
}
