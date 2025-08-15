namespace FitnessTracker.DataAccess.Entities;

public class ExerciseEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = String.Empty;
    public List<SetEntity> Sets { get; set; } = new();
}