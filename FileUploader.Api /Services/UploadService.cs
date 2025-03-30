namespace FileUploader.Api.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Database;
using Dtos;
using Exceptions;
using Models;
using Microsoft.EntityFrameworkCore;


using Folder = Models.Folder;
public class UploadService {
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<UploadService> _logger;
    private readonly ApplicationDbContext _context;

    public UploadService(Cloudinary cloudinary, ILogger<UploadService> logger , ApplicationDbContext context) {
        _cloudinary = cloudinary;
        _context = context;
        _logger = logger;
    }
    public async Task<FileItemDto> UploadFile(IFormFile file, int folderId, string userName)
    {

        var folder = await _context.Folders
            .FirstOrDefaultAsync(f => f.Id == folderId && f.Username == userName);

        if (folder == null)
        {
            throw new NotFoundException("Folder not found.");
        }

        var fileDescription = new FileDescription(file.FileName, file.OpenReadStream());
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        RawUploadParams uploadParams;
        switch (fileExtension)
        {
            case ".jpg":
            case ".jpeg":
            case ".png":
            case ".gif":
                uploadParams = new ImageUploadParams
                {
                    File = fileDescription,
                    UseFilename = true,
                    Overwrite = false
                };
                break;
            case ".mp4":
            case ".avi":
            case ".mov":
                uploadParams = new VideoUploadParams
                {
                    File = fileDescription,
                    UseFilename = true,
                    Overwrite = false
                };
                break;
            default:
                uploadParams = new RawUploadParams
                {
                    File = fileDescription,
                    UseFilename = true,
                    Overwrite = false
                };
                break;
        }
        _logger.LogInformation("Uploading file: {FileName} to folder: {FolderId}", file.FileName, folderId);

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new BadRequestException(uploadResult.Error.Message);
        }

        _logger.LogInformation("File uploaded successfully: {FileName} to folder: {FolderId}", file.FileName, folderId);


        _logger.LogInformation("saving in db : ");
        var fileItem = new FileItem
        {
            Name = file.FileName,
            Url = uploadResult.SecureUrl.ToString(),
            Size = (int)file.Length,
            CreatedAt = DateTime.UtcNow,
            FolderId = folderId,
            Username = userName
        };
        _logger.LogInformation("Saved file item ");

        _context.Files.Add(fileItem);
        await _context.SaveChangesAsync();

        return new FileItemDto
        {
            Id = fileItem.Id,
            Name = fileItem.Name,
            Url = fileItem.Url,
            Size = fileItem.Size,
            CreatedAt = fileItem.CreatedAt,
            FolderId = fileItem.FolderId
        };
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
        await _context.SaveChangesAsync();

        return folder;
    }

    


}