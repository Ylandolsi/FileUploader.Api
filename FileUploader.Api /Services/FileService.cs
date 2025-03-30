namespace FileUploader.Api.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Database;
using Dtos;
using Exceptions;
using Models;
using Microsoft.EntityFrameworkCore;


using Folder = Models.Folder;
public class FileService {
    private readonly ILogger<FileService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly CloudinaryService _cloudinaryService;

    public FileService(CloudinaryService cloudinaryService, ILogger<FileService> logger , ApplicationDbContext context) {
        _cloudinaryService = cloudinaryService;
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

        var uploadResult = await _cloudinaryService.UploadFileAsync(file);


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

    public async Task DeleteFile(int fileId, string userName)
    {
        _logger.LogInformation("deletefile with {fileId} and {userName}" , fileId , userName) ;
        var file = await _context.Files
            .FirstOrDefaultAsync(f => f.Id == fileId && f.Username == userName);

        _logger.LogInformation("id of the query = {file.Id}" , file.Id);

        if (file == null)
        {
            throw new NotFoundException("File not found.");
        }
        if (!string.IsNullOrEmpty(file.Url))
        {
            var uri = new Uri(file.Url);
            var pathSegments = uri.AbsolutePath.Split('/');
            var filename = pathSegments[pathSegments.Length - 1];
            var publicId = filename ; // if the file dosent have extension (exp:  .png ) 
            if ( filename.Contains('.'))
                publicId = filename.Substring(0, filename.LastIndexOf('.'));
                
            _logger.LogInformation("public id : = {publicId}" , publicId);
            _logger.LogInformation("deleting file from cloudinary");
            await _cloudinaryService.DeleteFileAsync(publicId);
            
            _logger.LogInformation("File deleted from cloudinary");
        }
        else {
            throw new InvalidOperationException("File URL is null or empty.");
        }
        
        _context.Files.Remove(file);
        await _context.SaveChangesAsync();
        
    }


    

    public async Task <string> ShareFile ( int fileId , string userName  , int duration ){
        var file = await _context.Files
            .FirstOrDefaultAsync(f => f.Id == fileId && f.Username == userName);

        if (file == null)
        {
            throw new NotFoundException("File not found.");
        }

        var tokenShare = Guid.NewGuid().ToString();
        _logger.LogInformation("tokenshare" , tokenShare);
        var expiry = DateTime.UtcNow.AddDays(duration);
        var sharedFile = new SharedFile
        {
            Token = tokenShare,
            Url = file.Url,
            ExpiresAt = expiry ,
            SharedBy = userName
        };
        _context.SharedFiles.Add(sharedFile);
        await _context.SaveChangesAsync();

        _logger.LogInformation("file shared");
        return tokenShare;
    }

    public string TransferToUrlDownload (string url)
    {
        // Add fl_attachment parameter to the URL to force download

        var uri = new Uri(url);
        string modifiedUrl;
        
        if (uri.Host.Contains("cloudinary.com"))
        {
            var segments = uri.AbsolutePath.Split('/'); // without host (website name )
            var lastSegmentIndex = segments.Length - 1;
            
            var scheme = uri.Scheme; 
            var host = uri.Host;
            var path = string.Join("/", segments.Take(lastSegmentIndex-1));
            var filename = segments[lastSegmentIndex];

            var beforeLast = segments[lastSegmentIndex -1] ;
            
            // https://res.cloudinary.com/dy1k8g2by/image/upload/v1743353500/fl_attachment/503c3b7f-16e9-4e90-b280-86f0527ba10e_e7wwon.jpg
            
            modifiedUrl = $"{scheme}://{host}{path}/fl_attachment/{beforeLast}/{filename}";
            return modifiedUrl;
        }

        else
        {
            throw new InvalidOperationException("URL is not from Cloudinary.");
        }
    }

    public async Task<string> DonwloadFile(int fileId, string userName)
    {
        var file = await _context.Files
            .FirstOrDefaultAsync(f => f.Id == fileId && f.Username == userName);

        if (file == null)
        {
            throw new NotFoundException("File not found.");
        }

        if (!string.IsNullOrEmpty(file.Url))
        {
            _logger.LogInformation("file url not null");
            return TransferToUrlDownload(file.Url);


        }
        
        throw new InvalidOperationException("File URL is null or empty.");

    
    }

    public async Task<string> GetSharedFile(string token)
    {
        var sharedFile = await _context.SharedFiles
            .FirstOrDefaultAsync(sf => sf.Token == token);

        if (sharedFile == null || sharedFile.ExpiresAt < DateTime.UtcNow)
        {
            throw new NotFoundException("Shared file not found or expired.");
        }

        if (!string.IsNullOrEmpty(sharedFile.Url))
        {
            _logger.LogInformation("file url not null");
            return TransferToUrlDownload(sharedFile.Url);


        }
        
        throw new InvalidOperationException("File URL is null or empty.");
    }




    


}