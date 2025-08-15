using FitnessTracker.Domain.Models;

namespace FitnessTracker.Domain.DataFilters;

public record WorkoutFilter(WorkoutType? Type, DateTime? CreatedAt, TimeSpan? Duration, string? SortItem, string? SortOrder, int PageNumber, int PageSize);