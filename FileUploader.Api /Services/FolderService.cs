namespace FileUploader.Api.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FileUploader.Api.Database;
using FileUploader.Api.Dtos;
using FileUploader.Api.Exceptions;
using FileUploader.Api.Models;
using Microsoft.EntityFrameworkCore;

using Folder = Models.Folder;
public class FolderService
{
    private readonly ILogger<FolderService> _logger;
    private readonly ApplicationDbContext _context;

    public FolderService( ILogger<FolderService> logger, ApplicationDbContext context)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FolderDto> CreateFolder(CreateFolderDto createFolderDto, string userName)
    {
        if (createFolderDto.ParentFolderId == null)
        {
            throw new InvalidOperationException("Parent folder ID is required.");
        }
        var parentFolder = await _context.Folders
            .FirstOrDefaultAsync(f => f.Id == createFolderDto.ParentFolderId && f.Username == userName);
        
        if (parentFolder == null)
        {
            throw new NotFoundException("Parent folder not found or you don't have permission to access it.");
        }
        var folder = new Folder
        {
            Name = createFolderDto.Name,
            Username = userName,
            ParentFolderId = createFolderDto.ParentFolderId
        };

        _context.Folders.Add(folder);
        try 
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error saving folder to the database");
            throw new InvalidOperationException("An error occurred while creating the folder.");
        }
        


        var folderDto = new FolderDto
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentFolderId = folder.ParentFolderId
        };

        return folderDto;
    }

    public async Task DeleteFolder(int folderId, string userName)
    {
        var folder = await _context.Folders
            .FirstOrDefaultAsync(f => f.Id == folderId && f.Username == userName);

        if (folder == null)
        {
            throw new NotFoundException("Folder not found.");
        }

        _context.Folders.Remove(folder);
        try 
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error deleting folder from the database");
            throw new InvalidOperationException("An error occurred while deleting the folder.");
        }
    }

    public async Task<List<FolderDto>> GetSubFolders(int folderId, string userName)
    {
        var folder = await _context.Folders
            .FirstOrDefaultAsync(f => f.Id == folderId && f.Username == userName);

        if (folder == null)
        {
            throw new NotFoundException("Folder not found or you don't have permission to access it.");
        }

        var subfolders = await _context.Folders
            .Where(f => f.ParentFolderId == folderId && f.Username == userName)
            .Select(f => new FolderDto
            {
                Id = f.Id,
                Name = f.Name,
                ParentFolderId = f.ParentFolderId
            })
            .ToListAsync();

        return subfolders;
    }


    public async Task<List<FolderDto>> GetFoldersAtRoot(string username )
    {
        // without root 
        var folders = await _context.Folders.Where(f=> f.ParentFolderId == null && f.Username == username ).ToListAsync();
        
        var ret = new List<FolderDto>();
        foreach (var folder in folders)
        {
            ret.Add(new FolderDto
            {
                Id = folder.Id,
                Name = folder.Name,
                ParentFolderId = folder.ParentFolderId ,
                
            });
            
        }
    
        return ret;
    }
    
    

    


}