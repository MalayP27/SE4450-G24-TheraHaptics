using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class PatientController : MonoBehaviour
{
    public static PatientView patientView;
    public static PatientModel patientModel;
    public static int currentExerciseIndex = 0;

    //HARDWARE Setup --------------------------------------------
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isRunning = true;

    private string serverIP = "127.0.0.1"; // Match your Python server IP
    private int serverPort = 9090;         // Match your Python server port

    private Queue<string> predictionQueue = new Queue<string>(); // Stores received predictions
    private const int MaxPredictions = 64; // Number of values to evaluate over 5 seconds (adjust if needed)

    private void Awake()
    {
        patientView = GetComponent<PatientView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
        StartCoroutine(ProcessPredictions()); // Start averaging every 5 seconds
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, serverPort);
            stream = client.GetStream();
            Debug.Log("✅ Connected to Python server");

            // Start a thread to receive data
            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Connection failed: " + e.Message);
        }
    }

     void ReceiveData()
    {
        byte[] buffer = new byte[1024];

        while (isRunning)
        {
            try
            {
                if (stream.CanRead)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        //Debug.Log(receivedMessage);
                        lock (predictionQueue)
                        {
                            if (predictionQueue.Count >= MaxPredictions)
                            {
                                predictionQueue.Dequeue(); // Remove oldest value if queue is full
                            }
                            predictionQueue.Enqueue(receivedMessage); // Store new prediction
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Error receiving data: " + e.Message);
                break;
            }
        }
    }

    private IEnumerator ProcessPredictions()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(5.0f); // Process every 5 seconds

            string finalPrediction = GetMostFrequentGesture();
            Debug.Log(finalPrediction);
        }
    }

    private string GetMostFrequentGesture()
    {
        lock (predictionQueue)
        {
            if (predictionQueue.Count == 0)
                return "none";

            return predictionQueue
                   .GroupBy(g => g)  // Group by gesture type
                   .OrderByDescending(g => g.Count()) // Sort by frequency
                   .First().Key; // Return most frequent
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        receiveThread?.Abort();
        stream?.Close();
        client?.Close();
    }

    //HARDWARE Setup End --------------------------------------------

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
                    {
                        // Get the current scene
                        Scene currentScene = SceneManager.GetActiveScene();

                        // If the current scene is PatientStartSession, set the exercise program
                        if (currentScene.name == "PatientStartSession")
                        {
                            patientView.SetExerciseProgram(exercisePrograms[0]);
                        }

                        // If the current scene is PatientExercise, set the current exercise
                        if (currentScene.name == "PatientExercise" && exercisePrograms[0].exercises.Count > 0)
                        {
                            if(currentExerciseIndex >= exercisePrograms[0].exercises.Count)
                            {
                                patientView.EndSession();
                                return;
                            }
                            
                            patientView.SetCurrentExercise(exercisePrograms[0].exercises[currentExerciseIndex]);
                        }
                    }
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