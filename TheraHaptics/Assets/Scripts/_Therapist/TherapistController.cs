using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using UnityEngine.Networking;

public class TherapistController : MonoBehaviour
{
    public static TherapistView therapistView;

    private void Awake()
    {
        // Assume that the TherapistView component is attached to the same GameObject.
        therapistView = GetComponent<TherapistView>();
    }

    [System.Serializable]
    public class AddPatientRequest
    {
        public string therapistId;
        public string firstName;
        public string lastName;
        public string emailAddress;
        public string diagnosis;
    }

    public static async void AddPatient(string therapistId, string firstName, string lastName, string emailAddress, string diagnosis)
    {
        // Input validation: Check for empty strings.
        if (string.IsNullOrEmpty(therapistId) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(emailAddress) || string.IsNullOrEmpty(diagnosis))
        {
            therapistView.HandleAddPatientError("All fields are required.");
            return;
        }

        // Create the JSON payload with the correct property names.
        AddPatientRequest request = new AddPatientRequest
        {
            firstName = firstName,
            lastName = lastName,
            emailAddress = emailAddress,
            diagnosis = diagnosis,
            therapistId = therapistId
        };

        string jsonPayload = JsonUtility.ToJson(request);
        Debug.Log("JSON Payload: " + jsonPayload);

        using (var client = new HttpClient())
        {
            try
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("http://localhost:5089/api/therapist/newPatient", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("Add patient successful. Response: " + responseBody);
                    therapistView.CloseAddPatientScreen();                
                }
                else
                {
                    therapistView.HandleAddPatientError("Add patient failed: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex) //
            {
                therapistView.HandleAddPatientError("Error during add patient: " + ex.Message);
            }
        }
    }

    public static IEnumerator GetPatientListCoroutine()
    {
        string therapistId = RegisterController.TherapistId; // Ensure this is set!
        string url = $"http://localhost:5089/api/therapist/getPatientList/{therapistId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseBody = request.downloadHandler.text;
                Debug.Log("Get patient list successful. Response: " + responseBody);
                therapistView.HandleGetPatientListSuccess(responseBody);
            }
            else
            {
                therapistView.HandleGetPatientListError("Get patient list failed: " + request.error);
            }
        }
    }
}