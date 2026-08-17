using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SKDJK.Models;
using SKDJK.Models.enums;

namespace SKDJK.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.RoleName)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsUnicode(false);
            builder.Property(r => r.Description)
                .IsRequired(false)
                .HasColumnType("nvarchar")
                .HasMaxLength(255);
        }
    }
}
