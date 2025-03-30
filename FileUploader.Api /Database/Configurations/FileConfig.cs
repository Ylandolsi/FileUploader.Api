using FileUploader.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace FileUploader.Api.Database.Configurations;
public class FileConfig : IEntityTypeConfiguration<FileItem>
{
    public void Configure(EntityTypeBuilder<FileItem> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired(); 

        builder.Property(f => f.Url)
            .IsRequired();

        builder.Property(f => f.Size)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', NOW())");


        builder.HasOne(fi => fi.Folder) 
            .WithMany(fo => fo.Files) 
            .HasForeignKey(fi => fi.FolderId) 
            .OnDelete(DeleteBehavior.Cascade); 


        builder.HasOne(fi => fi.User)
            .WithMany(u => u.Files)
            .HasForeignKey(fi => fi.Username)
            .OnDelete(DeleteBehavior.Cascade);
    }
}