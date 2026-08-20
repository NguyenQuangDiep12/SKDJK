using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;

namespace SKDJK.Data.Configurations
{
    public class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
    {
        public void Configure(EntityTypeBuilder<UserAnswer> builder)
        {
            builder.ToTable("useranswers");
            builder.HasKey(ua => ua.Id);
            builder.Property(ua => ua.Id)
                .ValueGeneratedOnAdd();

            builder.Property(ua => ua.TextAnswer)
                .IsRequired(false)
                .HasMaxLength(200);

            builder.HasOne(ua => ua.Answer)
                .WithMany(a => a.UserAnswers)
                .HasForeignKey(ua => ua.AnswerId)
                .IsRequired(false)
                .HasConstraintName("FK_UserAnswer_Answer")
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(ua => ua.Question)
                .WithMany(q => q.UserAnswers)
                .HasForeignKey(ua => ua.QuestionId)
                .HasConstraintName("FK_UserAnswer_Question")
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(ua => ua.TestResult)
                .WithMany(t => t.UserAnswers)
                .HasForeignKey(ua => ua.TestResultId)
                .HasConstraintName("FK_UserAnswer_TestResult")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
    
