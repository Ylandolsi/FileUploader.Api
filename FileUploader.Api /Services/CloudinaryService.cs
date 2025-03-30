using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FileUploader.Api.Exceptions;

namespace FileUploader.Api.Services;
public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryService> _logger;    
    public CloudinaryService(Cloudinary cloudinary , ILogger<CloudinaryService> logger)
    {
        _logger = logger;
        _cloudinary = cloudinary;
    }

    public async Task<DeletionResult> DeleteFileAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }
    public async Task<UploadResult> UploadFileAsync(IFormFile file)
    {
        
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
        _logger.LogInformation("Uploading file: {FileName} ", file.FileName);

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new BadRequestException(uploadResult.Error.Message);
        }

        _logger.LogInformation("File uploaded successfully: {FileName}", file.FileName );

        return uploadResult;
    }

   
}