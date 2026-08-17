using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;

namespace SKDJK.Data.Configurations
{
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.ToTable("topics");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasMaxLength(100);
            builder.Property(t => t.Level)
                .IsRequired()
                .HasColumnType("varchar")
                .HasMaxLength(50);
            builder.Property(t => t.Description)
            .IsRequired(false)
            .HasColumnType("nvarchar")
            .HasMaxLength(255);

            builder.HasOne(t => t.Language)
                .WithMany(l => l.Topics)
                .HasForeignKey(t => t.LanguageId)
                .HasConstraintName("FK_Language_Topic")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
