namespace FitnessTracker.Domain.Models;

public class Exercise
{
    public Exercise() { }
    private Exercise(string id, string name, List<Set> sets)
    {
        Name = name;
        Sets = sets;
        Id = id;
    }
    public string Id { get; set; }
    public string Name { get; set; } 
    public List<Set> Sets { get; set; }

    public static Exercise Create(string name, List<Set> sets)
    {
        return new Exercise(Guid.NewGuid().ToString(), name, sets);
    }

}