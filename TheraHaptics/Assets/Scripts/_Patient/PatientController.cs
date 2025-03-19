using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class PatientController : MonoBehaviour
{
    public static PatientView patientView;
    public static PatientModel patientModel;

    private void Awake()
    {
        patientView = GetComponent<PatientView>();
    }

    [System.Serializable]
    public class PatientResponse
    {
        public string patientId;
        public string firstName;
        public string lastName;
    }

    public static IEnumerator GetPatientCoroutine()
    {
        string patientId = LoginController.patientModel.GetPatientID(); // Ensure this is set correctly
        Debug.Log("PatientController: " + patientId);
        string url = $"http://localhost:5089/api/patient/getPatient/{patientId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Get patient successful. Response: " + request.downloadHandler.text);
                string responseBody = request.downloadHandler.text;
                PatientResponse patient = JsonUtility.FromJson<PatientResponse>(responseBody);
                if (patient != null)
                {
                    string fullName = patient.firstName + " " + patient.lastName;
                    if (patientView != null)
                        patientView.setWelcomeMessage(fullName);
                }
                else
                {
                    Debug.LogError("Failed to parse patient information.");
                }
            }
            else
            {
                Debug.LogError("Error fetching patient data: " + request.error);
            }
        }
    }

    public static async void GetExerciseProgram()
    {
        string patientId = LoginController.patientModel.GetPatientID();
        Debug.Log("PatientController: " + patientId);
        string url = $"http://localhost:5089/api/patient/getExerciseProgramsByPatientId/{patientId}";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("Exercise program response: " + responseBody);

                    // Wrap the JSON array in an object since JsonUtility doesn't support top-level arrays
                    string wrappedJson = "{\"items\":" + responseBody + "}";
                    Debug.Log("Wrapped JSON: " + wrappedJson);
                    List<ExerciseProgram> exercisePrograms = JsonUtility.FromJson<Wrapper<ExerciseProgram>>(wrappedJson).items;

                    if (patientView != null && exercisePrograms.Count > 0)
                        patientView.SetExerciseProgram(exercisePrograms[0]);
                }
                else
                {
                    Debug.LogError("Failed to fetch exercise program. Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception during API call: " + ex.Message);
            }
        }
    }

    [System.Serializable]
    public class Exercise
    {
        public string exerciseId;
        public string name;
        public string instructions;
        public int targetReps;
        public int targetDuration;
        public string intensity;
    }

    [System.Serializable]
    public class ExerciseProgram
    {
        // Updated to match the expected JSON structure:
        public string programID;  // Changed from programId to programID
        public string name;
        public string patientId;  // Added to match the JSON
        public List<Exercise> exercises;
        public string startDate;
        public string endDate;
        public string planGoals;
        public string intensity;
        public int estimatedTime;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }
}