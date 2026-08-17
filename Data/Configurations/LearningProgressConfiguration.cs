using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;
using SKDJK.Models.enums;

namespace SKDJK.Data.Configurations
{
    public class LearningProgressConfiguration : IEntityTypeConfiguration<LearningProgress>
    {
        public void Configure(EntityTypeBuilder<LearningProgress> builder)
        {
            builder.ToTable("learningprogresses");
            builder.HasKey(lp => lp.Id);
            builder.Property(lp => lp.Id)
                .ValueGeneratedOnAdd();

            builder.Property(lp => lp.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("varchar")
                .HasMaxLength(20)
                .HasDefaultValue(LearningStatus.NOTSTARTED);
            builder.Property(lp => lp.CompletionPercent)
                .IsRequired()
                 .HasPrecision(4, 1)
                .HasDefaultValue(0);
            builder.Property(lp => lp.LastStudyAt)
                .IsRequired(false)
                .HasColumnType("Datetime2");

            builder.HasOne(lp => lp.User)
                .WithMany(u => u.LearningProgresses)
                .HasForeignKey(lp => lp.UserId)
                .HasConstraintName("FK_LearningProgress_User")
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(lp => lp.Lesson)
                .WithMany(l => l.LearningProgresses)
                .HasForeignKey(lp => lp.LessonId)
                .HasConstraintName("FK_LearningProgress_Lesson")
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
