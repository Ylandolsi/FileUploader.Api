using FileUploader.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RefreshConfig : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id).ValueGeneratedOnAdd();
        builder.Property(rt => rt.RefreshToken).IsRequired();
        builder.Property(rt => rt.UserName).IsRequired();

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserName)
            .HasPrincipalKey(u => u.Username)
            .OnDelete(DeleteBehavior.Cascade);
    }
}