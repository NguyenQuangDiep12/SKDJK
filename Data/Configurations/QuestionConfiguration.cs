using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;

namespace SKDJK.Data.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("questions");
            builder.HasKey(q => q.Id);
            builder.Property(q => q.Id)
                .ValueGeneratedOnAdd();

            builder.Property(q => q.Content)
                .IsRequired()
                .HasColumnType("text");
            builder.Property(q => q.ImageUrl)
                .IsRequired(false)
                .HasColumnType("varchar")
                .HasMaxLength(255);
            builder.Property(q => q.QuestionType)
                .IsRequired()
                .HasColumnType("varchar")
                .HasConversion<string>()
                .HasMaxLength(20);
            builder.Property(q => q.AudioUrl)
                .IsRequired(false)
                .HasColumnType("varchar")
                .HasMaxLength(255);

            builder.HasOne(q => q.Test)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TestId)
                .HasConstraintName("FK_Question_Test")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
