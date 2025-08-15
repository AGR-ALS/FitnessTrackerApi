namespace FitnessTracker.Domain.Models;

public class Set
{
    public Set() { }
    private Set(string id, int reps, double weight)
    {
        Reps = reps;
        Weight = weight;
        Id = id;
    }

    public string Id { get; set; }
    public int Reps { get; set; }
    public double Weight { get; set; }

    public static Set Create(int reps, double weight)
    {
        return new Set(Guid.NewGuid().ToString(), reps, weight);
    }
    
}