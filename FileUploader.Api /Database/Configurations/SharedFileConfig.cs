namespace FileUploader.Api.Database.Configurations;

using FileUploader.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class SharedFileConfiguration : IEntityTypeConfiguration<SharedFile>
{
    public void Configure(EntityTypeBuilder<SharedFile> builder)
    {
        builder.HasKey(sf => sf.Token);
        builder.Property(sf => sf.Token).IsRequired().HasMaxLength(50);
        builder.Property(sf => sf.ExpiresAt).IsRequired();
        builder.Property(sf => sf.SharedBy).IsRequired().HasMaxLength(50);
      
    }
}