using FitnessTracker.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.DataAccess.Configurations;

public class WorkoutsConfiguration : IEntityTypeConfiguration<WorkoutEntity>
{
    public void Configure(EntityTypeBuilder<WorkoutEntity> builder)
    {
        builder.ToTable("Workouts");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).HasMaxLength(36).IsRequired();
        builder.Property(w => w.UserId).HasMaxLength(36).IsRequired();
        builder.Property(w => w.Type).IsRequired();
        builder.Property(w => w.Duration).IsRequired();
        builder.Property(w=>w.CaloriesBurned).IsRequired();
        builder.Property(w => w.ProgressPhotos).IsRequired();
        builder.Property(w=>w.WorkoutDate).IsRequired();
        builder.Property(w=>w.Title).HasMaxLength(100).IsRequired();
        builder.HasOne(w => w.User).WithMany().HasForeignKey(w => w.UserId);
        builder.OwnsMany(w => w.Exercises, ex =>
        {
            ex.ToTable("Exercises");
            ex.WithOwner().HasForeignKey("WorkoutId");
            ex.Property<string>("Id").IsRequired().ValueGeneratedNever().HasMaxLength(36);
            ex.Property<string>("Name").IsRequired().HasMaxLength(100);
            ex.HasKey("Id");
            ex.OwnsMany(e => e.Sets, set =>
            {
                set.ToTable("Sets");
                set.WithOwner().HasForeignKey("ExerciseId");
                set.Property<string>("Id").IsRequired().ValueGeneratedNever().HasMaxLength(36);
                set.HasKey("Id");
            });
        });
    }
}