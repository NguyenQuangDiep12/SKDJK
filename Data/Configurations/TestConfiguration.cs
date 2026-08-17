using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;

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

            builder.HasOne(t => t.Lesson)
                .WithMany(l => l.Tests)
                .HasForeignKey(t => t.LessonId)
                .HasConstraintName("FK_Test_Lesson")
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
