using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
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

                // Send the request and wait for it to complete
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Get patient successful. Response: " + request.downloadHandler.text);
                    string responseBody = request.downloadHandler.text;
                    PatientResponse patient = JsonUtility.FromJson<PatientResponse>(responseBody);
                    if (patient != null)
                    {
                        string fullName = patient.firstName + " " + patient.lastName;

                        // Find the PatientView instance and update the welcome message
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
    }
