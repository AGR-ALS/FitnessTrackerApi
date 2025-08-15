namespace FitnessTrackerApi.Contracts.WorkoutsContracts;

public record GetExerciseResponse(string Id, string Name, List<GetSetResponse> Sets);