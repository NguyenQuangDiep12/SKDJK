using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;

namespace SKDJK.Data.Configurations
{
    public class VocabularyConfiguration : IEntityTypeConfiguration<Vocabulary>
    {
        public void Configure(EntityTypeBuilder<Vocabulary> builder)
        {
            builder.ToTable("vocabularies");
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                .ValueGeneratedOnAdd();

            builder.Property(v => v.Word)
                .IsRequired()
                .HasColumnType("varchar")
                .HasMaxLength(100);
            builder.Property(v => v.Meaning)
                .IsRequired(false)
                .HasColumnType("nvarchar")
                .HasMaxLength(255);
            builder.Property(v => v.Pronunciation)
                .IsRequired(false)
                .HasColumnType("nvarchar")
                .HasMaxLength(100);
            builder.Property(v => v.Example)
                .IsRequired(false)
                .HasColumnType("text");
            builder.Property(v => v.AudioUrl)
                .IsRequired(false)
                .HasColumnType("varchar")
                .HasMaxLength(255);

            // Cau hinh lien ket thuc the chi tiet
            builder.HasOne(v => v.Lesson)
                .WithMany(l => l.Vocabularies)
                .HasForeignKey(v => v.LessonId)
                .HasConstraintName("FK_Vocabulary_Lesson")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
