namespace Server.Models;

public class MongoDBSettings {

    public string ConnectionURI { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string PlaylistCollectionName { get; set; } = null!;
    public string ProductKeyCollectionName { get; set; } = null!;
    public string UserCollectionName { get; set; } = null!;
    public string TherapistCollectionName { get; set; } = null!;
    public string PatientCollectionName { get; set; } = null!;
    public string ExerciseCollectionName { get; set; } = null!;
    public string ExerciseProgramCollectionName { get; set; } = null!;
    public string PainReportCollectionName { get; set; } = null!;
    public string ProgressReportCollectionName { get; set; } = null!;
    public string SessionCollectionName { get; set; } = null!;
    public string GoalCollectionName { get; set; } = null!;
}