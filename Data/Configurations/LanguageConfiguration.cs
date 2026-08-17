using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;

namespace SKDJK.Data.Configurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("languages");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id)
                .ValueGeneratedOnAdd();

            builder.Property(l => l.Name)
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasDefaultValue("English")
                .HasMaxLength(50);
            builder.Property(l => l.Code)
                .IsRequired()
                .HasColumnType("varchar")
                .HasMaxLength(10);
            builder.HasIndex(l => l.Code)
                .IsUnique();
            builder.Property(l => l.Description)
                .IsRequired(false)
                .HasColumnType("text");
        }
    }
}
