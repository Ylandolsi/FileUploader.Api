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
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<FolderService> _logger;
    private readonly ApplicationDbContext _context;

    public FolderService(Cloudinary cloudinary, ILogger<FolderService> logger, ApplicationDbContext context)
    {
        _cloudinary = cloudinary;
        _context = context;
        _logger = logger;
    }

    public async Task<Folder> CreateFolder(CreateFolderDto createFolderDto, string userName)
    {
        var folder = new Folder
        {
            Name = createFolderDto.Name,
            Username = userName,
            ParentFolderId = createFolderDto.ParentFolderId,
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

        return folder;
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

    public async Task<FolderContentsDto> GetFolderContents(int folderId, string userName)
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
            })
            .ToListAsync();

        var files = await _context.Files
            .Where(f => f.FolderId == folderId && f.Username == userName)
            .Select(f => new FileItemDto
            {
                Id = f.Id,
                Name = f.Name,
                Size = f.Size,
                Url = f.Url,
                CreatedAt = f.CreatedAt,
                FolderId = f.FolderId
            })
            .ToListAsync();

        return new FolderContentsDto
        {
            CurrentFolder = new FolderDto
            {
                Id = folder.Id,
                Name = folder.Name,
                ParentFolderId = folder.ParentFolderId
            },
            Subfolders = subfolders,
            Files = files
        };
    }


    public async Task<List<FolderDto>> GetPathToFolderOptimized(int folderId, string userName)
    {
        // depth 5 at max 
        var folderWithAncestors = await _context.Folders
            .Where(f => f.Username == userName)
            .Include(f => f.ParentFolder.ParentFolder.ParentFolder.ParentFolder.ParentFolder)
            .FirstOrDefaultAsync(f => f.Id == folderId);
        
        if (folderWithAncestors == null)
        {
            throw new NotFoundException("Folder not found or you don't have permission to access it.");
        }
        
        var path = new List<FolderDto>();
        var current = folderWithAncestors;
        
        while (current != null)
        {
            path.Add(new FolderDto
            {
                Id = current.Id,
                Name = current.Name,
                ParentFolderId = current.ParentFolderId
            });
            
            current = current.ParentFolder;
        }
        
        path.Reverse();
        return path;
    }


    public async Task<IEnumerable<FileItemDto>> GetFilesInFolder(int folderId, string userName)
    {
        var folder = await _context.Folders
            .FirstOrDefaultAsync(f => f.Id == folderId && f.Username == userName);

        if (folder == null)
        {
            throw new NotFoundException("Folder not found or you don't have permission to access it.");
        }


        var files = await _context.Files
            .Where(f => f.FolderId == folderId && f.Username == userName)
            .Select(f => new FileItemDto
            {
                Id = f.Id,
                Name = f.Name,
                Size = f.Size,
                Url = f.Url,
                CreatedAt = f.CreatedAt,
                FolderId = f.FolderId
            })
            .ToListAsync();

        return files ;
    }

}