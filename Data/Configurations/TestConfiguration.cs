using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;
using SKDJK.Models.enums;

namespace SKDJK.Data.Configurations
{
    public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.ToTable("tests");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasMaxLength(255);
            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasColumnType("nvarchar")
                .HasMaxLength(255);
            builder.Property(x => x.DurationMinutes)
                .IsRequired();
            builder.Property(x => x.Format)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("varchar")
                .HasMaxLength(20);
            builder.Property(x => x.Mode)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("varchar")
                .HasMaxLength(20);
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(t => t.Lesson)
                .WithMany(l => l.Tests)
                .HasForeignKey(t => t.LessonId)
                .HasConstraintName("FK_Test_Lesson")
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
