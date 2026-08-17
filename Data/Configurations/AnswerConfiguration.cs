using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;

namespace SKDJK.Data.Configurations
{
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.ToTable("answers");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Content)
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasMaxLength(255);

            builder.Property(a => a.IsCorrect)
                .IsRequired();

            builder.HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .HasConstraintName("FK_Answer_Question")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
