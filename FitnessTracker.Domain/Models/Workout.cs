namespace FitnessTracker.Domain.Models;

public class Workout : IDocument
{
    public Workout() { }
    private Workout(string id, DateTime createdAt, string userId, string title, WorkoutType workoutType,
        List<Exercise> exercises, TimeSpan duration, int caloriesBurned, List<string> progressPhotos,
        DateTime workoutDate)
    {
        Id = id;
        CreatedAt = createdAt;
        UserId = userId;
        Title = title;
        Type = workoutType;
        Exercises = exercises;
        Duration = duration;
        CaloriesBurned = caloriesBurned;
        ProgressPhotos = progressPhotos;
        WorkoutDate = workoutDate;
    }

    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }
    public WorkoutType Type { get; set; }
    public List<Exercise> Exercises { get; set; }
    public TimeSpan Duration { get; set; }
    public int CaloriesBurned { get; set; }
    public List<string> ProgressPhotos { get; set; }
    public DateTime WorkoutDate { get; set; }

    public static Workout Create(DateTime createdAt, string userId, string title, WorkoutType workoutType,
        List<Exercise> exercises, TimeSpan duration, int caloriesBurned, List<string> progressPhotos,
        DateTime workoutDate)
    {
        return new Workout(Guid.NewGuid().ToString(), createdAt, userId, title, workoutType, exercises, duration, caloriesBurned,
            progressPhotos, workoutDate);
    }

    public static Workout RestoreFromEntity(string id, DateTime createdAt, string userId, string title, WorkoutType workoutType,
        List<Exercise> exercises, TimeSpan duration, int caloriesBurned, List<string> progressPhotos,
        DateTime workoutDate)
    {
        return new Workout(id, createdAt, userId, title, workoutType, exercises, duration, caloriesBurned,
            progressPhotos, workoutDate);
    }
}