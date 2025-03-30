using FileUploader.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace FileUploader.Api.Database.Configurations;
public class FolderConfig : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasOne(f => f.ParentFolder) 
            .WithMany(f => f.ChildFolders) 
            .HasForeignKey(f => f.ParentFolderId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasMany(fo => fo.Files)
            .WithOne(fi => fi.Folder) 
            .HasForeignKey(fi => fi.FolderId) 
            .OnDelete(DeleteBehavior.Cascade); 


        builder.HasOne(f => f.User)
            .WithMany(u => u.Folders)
            .HasForeignKey(f => f.Username)
            .OnDelete(DeleteBehavior.Cascade);   // Delete folders when the user is deleted

    }
}