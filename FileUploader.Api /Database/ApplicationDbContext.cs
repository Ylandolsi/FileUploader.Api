namespace FileUploader.Api.Database;

using Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly( typeof(ApplicationDbContext).Assembly );
        base.OnModelCreating(modelBuilder); 
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserRefreshToken> RefreshTokens { get; set; } = null!;  

    public DbSet<Folder> Folders { get; set; } = null!;
    public DbSet<FileItem> Files { get; set; } = null!;

    public DbSet<SharedFile> SharedFiles { get; set; } = null!;
}