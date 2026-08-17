using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;

namespace SKDJK.Data.Configurations
{
    public class PronunciationResultConfiguration : IEntityTypeConfiguration<PronunciationResult>
    {
        public void Configure(EntityTypeBuilder<PronunciationResult> builder)
        {
            builder.ToTable("pronunciationresults");
            builder.HasKey(pr => pr.Id);
            builder.Property(pr => pr.Id)
                .ValueGeneratedOnAdd();

            builder.Property(pr => pr.Score)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.HasOne(pr => pr.User)
                .WithMany(u => u.PronunciationResults)
                .HasForeignKey(pr => pr.UserId)
                .HasConstraintName("FK_PronunciationResult_User")
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pr => pr.Vocabulary)
                .WithMany(v => v.PronunciationResults)
                .HasForeignKey(pr => pr.VocabularyId)
                .HasConstraintName("FK_PronunciationResult_Vocabulary")
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
