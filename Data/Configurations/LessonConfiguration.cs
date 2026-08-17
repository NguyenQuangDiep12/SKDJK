using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;

namespace SKDJK.Data.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.ToTable("lessons");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id)
                .ValueGeneratedOnAdd();

            builder.Property(l => l.Title)
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasMaxLength(255);
            builder.Property(l => l.Description)
                .IsRequired(false)
                .HasColumnType("nvarchar")
                .HasMaxLength(255);
            builder.Property(l => l.Content)
                .IsRequired()
                .HasColumnType("text");

            builder.HasOne(l => l.Topic)
                .WithMany(t => t.Lessons)
                .HasForeignKey(l => l.TopicId)
                .HasConstraintName("FK_Topic_Lesson")
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
