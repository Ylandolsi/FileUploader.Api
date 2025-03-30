using FileUploader.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfig : IEntityTypeConfiguration<User> 
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Username);
        builder.Property(x => x.Username).IsRequired().HasMaxLength(50);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(100);
        builder.Property(x => x.FirstName).IsRequired(false).HasMaxLength(50);
        builder.Property(x => x.LastName).IsRequired(false).HasMaxLength(50);


        builder.HasMany(x => x.Folders)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.Username)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Files)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.Username)
            .OnDelete(DeleteBehavior.Cascade);
    }
}