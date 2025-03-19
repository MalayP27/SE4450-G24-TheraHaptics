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
                    therapistView. TherapistDashboard();           
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

    public static IEnumerator GetPatientListDropdownCoroutine()
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
                therapistView.HandleGetPatientListDropdownSuccess(responseBody);
            }
            else
            {
                therapistView.HandleGetPatientListError("Get patient list failed: " + request.error);
            }
        }
    }

       public static IEnumerator GetPatientListDashboardCoroutine()
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
                therapistView.HandleGetPatientListDashboardSuccess(responseBody);
            }
            else
            {
                therapistView.HandleGetPatientListError("Get patient list failed: " + request.error);
            }
        }
    }

    
    public class TherapistResponse
    {
        public string therapistId;
        public string firstName;
        public string lastName;
    }
    public static void GetTherapist()
    {
        string therapistId = RegisterController.TherapistId; // Ensure this is set!
        // Adjust the URL to match the API route (no extra "getTherapist" segment needed).
        string url = $"http://localhost:5089/api/therapist/{therapistId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            // Start the request and wait for it to complete.
            var asyncOp = request.SendWebRequest();
            while (!asyncOp.isDone)
            {
                // Optionally, yield return null if using this in a coroutine.
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseBody = request.downloadHandler.text;
                Debug.Log("Get therapist successful. Response: " + responseBody);

                // Deserialize the JSON response into a TherapistResponse object.
                TherapistResponse therapist = JsonUtility.FromJson<TherapistResponse>(responseBody);
                if (therapist != null)
                {
                    // Create the welcome message.
                    string welcomeMessage = "Hi " + therapist.firstName + " " + therapist.lastName;
                    // Set the welcome message in the UI.
                    therapistView.setWelcomeMessage(welcomeMessage);
                }
                else
                {
                    Debug.LogError("Failed to parse therapist information.");
                }
            }
            else
            {
        //        therapistView.HandleGetTherapistError("Get therapist failed: " + request.error);
            }
       }   
    }

        public static IEnumerator GetAllExercisesCoroutine()
        {
            // Corrected URL:
            string url = "http://localhost:5089/api/exercise/getAllExercises";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Accept", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseBody = request.downloadHandler.text;
                    Debug.Log("Get all exercises successful. Response: " + responseBody);
                    therapistView.HandleGetAllExercisesSuccess(responseBody);
                }
                else
                {
                    Debug.LogError("Get all exercises failed: " + request.error);
                }
            }
        }
        
    [Serializable]
     public class ExerciseDto
    {
        public string exerciseId;     // string: e.g., a MongoDB ObjectId (or empty if not available)
        public string Name;           // string: exercise name
        public string Instructions;   // string: instructions (optional, can be left empty)
        public int TargetReps;        // int: target repetitions
        public int TargetDuration;    // int: target duration in minutes
        public string Intensity;      // string: intensity (converted from numeric slider)
    }

    // DTO for the entire exercise program.
    [Serializable]
    public class ExerciseProgramCreateDto
    {
        public string Name;                  // string: e.g., "New Exercise Program"
        public string PatientId;             // string: selected patient's ID
        public List<ExerciseDto> Exercises;  // list of exercises
        public string StartDate;           
        public string EndDate;             
        public string PlanGoals;             // string: doctor's notes
        public string Intensity;             // string: overall intensity (converted from slider)
        public int EstimatedTime;            // int: total estimated time (e.g., sum of all target durations)
    }

    /// <summary>
    /// Submits the exercise program by creating a DTO and calling the API endpoint.
    /// </summary>
    public static async void SubmitExercisePlan(
        int numOfExercises,
        string[] exercisesInPlan, 
        float[] timeNums, 
        int[] repNums, 
        string startDateText, 
        string endDateText, 
        string intensity, 
        string planGoals, 
        string patientId,
        string[] exerciseIds)
    {
        // Build a list of exercises.
        List<ExerciseDto> exerciseList = new List<ExerciseDto>();
        Debug.Log(exerciseIds + "ersrs");
        for (int i = 0; i < numOfExercises; i++)
        {
            // Call the API to get the exercise details by ID.
            ExerciseDto exercise = await GetExerciseById(exerciseIds[i]);
            if (exercise != null)
            {
                exercise.exerciseId = exerciseIds[i];
                exercise.Name = exercisesInPlan[i];
                exercise.Instructions= "Instructions"; // Or obtain from a dedicated UI input
                exercise.TargetReps = repNums[i];
                exercise.TargetDuration = (int)timeNums[i];
                exercise.Intensity = intensity.ToString(); // Convert numeric intensity to string
                exerciseList.Add(exercise);
            }
        }

        // Parse start and end dates from input strings.
        // DateTime startDate = DateTime.Parse(startDateText);
        // DateTime endDate = DateTime.Parse(endDateText);
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(7);

        // Calculate estimated time as the sum of all target durations.
        int estimatedTime = 0;
        foreach (var t in timeNums)
        {
            estimatedTime += (int)t;
        }

        // Build the overall exercise program DTO.
        ExerciseProgramCreateDto programDto = new ExerciseProgramCreateDto
        {
            Name = "New Exercise Program", // Or obtain from a dedicated UI input
            PatientId = patientId,
            Exercises = exerciseList,
            StartDate = startDate.ToString("o"),
            EndDate = endDate.ToString("o"),
            PlanGoals = planGoals,
            Intensity = intensity.ToString(), // Overall intensity as string
            EstimatedTime = estimatedTime
        };

        // Serialize the DTO to JSON.
        string jsonPayload = JsonUtility.ToJson(programDto);
        Debug.Log("JSON Payload: " + jsonPayload);

        // Call the API endpoint.
        using (HttpClient client = new HttpClient())
        {
            try
            {
                StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("http://localhost:5089/api/exerciseprogram/createExerciseProgram", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("Exercise program created successfully. Response: " + responseBody);
                    // Optionally, trigger a success UI update.
                }
                else
                {
                    Debug.LogError("Failed to create exercise program: " + response.ReasonPhrase);
                    // Handle the error appropriately.
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception during API call: " + ex.Message);
            }
        }
    }

    private static async Task<ExerciseDto> GetExerciseById(string exerciseId)
    {
        string url = $"http://localhost:5089/api/exercise/getExercise/{exerciseId}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log("Get exercise by ID successful. Response: " + responseBody);
                    return JsonUtility.FromJson<ExerciseDto>(responseBody);
                }
                else
                {
                    Debug.LogError("Failed to get exercise by ID: " + response.ReasonPhrase);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception during API call: " + ex.Message);
                return null;
            }
        }
    }
}
