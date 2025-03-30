using FileUploader.Api.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FileUploader.Api.Database;

public class UserSaveInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is ApplicationDbContext context)
        {
            var newUsers = context.ChangeTracker.Entries<User>()
                .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                .Select(e => e.Entity)
                .ToList();

            foreach (var user in newUsers)
            {
                context.Folders.Add(new Models.Folder
                {
                    Name = "Root",
                    ParentFolderId = null,
                    Username = user.Username
                });
            }
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}