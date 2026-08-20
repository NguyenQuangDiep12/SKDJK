using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;

namespace SKDJK.Data.Configurations
{
    public class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
    {
        public void Configure(EntityTypeBuilder<TestResult> builder)
        {
            builder.ToTable("testresults");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Score)
                .IsRequired()
                .HasPrecision(5, 2);
            builder.Property(x => x.CorrectCount)
                .IsRequired()
                .HasColumnType("int")
                .HasDefaultValue(0);
            builder.Property(x => x.TotalQuestions)
                .IsRequired()
                .HasColumnType("int")
                .HasDefaultValue(0);
            builder.Property(x => x.SubmittedAt)
                .IsRequired(false)
                .HasColumnType("Datetime2");

            builder.HasOne(x => x.User)
                .WithMany(u => u.TestResults)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_TestResult_User")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Test)
                .WithMany(t => t.TestResults)
                .HasForeignKey(x => x.TestId)
                .HasConstraintName("FK_TestResult_Test")
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
