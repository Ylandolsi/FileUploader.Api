using FileUploader.Api.Dtos;
using FileUploader.Api.Exceptions;
using FileUploader.Api.Models;
using FileUploader.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Api.Controllers;


using JWTClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using InvalidOperationException = Exceptions.InvalidOperationException;
[ApiController]
[Route("api/[controller]")]

public class FileController : ControllerBase{

    private readonly FileService _fileService;
    private readonly ILogger<FileController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration; 

    public FileController(FileService fileService, ILogger<FileController> logger , 
        IWebHostEnvironment environment, IConfiguration configuration) {
        _fileService = fileService;
        _logger = logger;
        _environment = environment;
        _configuration = configuration;
    }


    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> Upload(IFormFile file, int folderId)
    {
        if (folderId <= 0) 
            return BadRequest(new { message = "Invalid folder ID" });
        if (file == null || file.Length == 0) 
            return BadRequest(new { message = "File is required" });
        if (file.Length > 5 * 1024 * 1024) 
            return BadRequest( new {message = "File size exceeds the 5 MB limit."});

        var currentUsernameClaim = User.FindFirst(JWTClaims.Name); 
        if (currentUsernameClaim == null)
            return Unauthorized(new { message = "User not authenticated" });
        var userName = currentUsernameClaim.Value;       
        try
        {
            var result = await _fileService.UploadFile(file, folderId, userName);
            return Ok(result);
        }
        catch (NotFoundException  ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, new { message = "An error occurred while uploading the file." });
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteFile( int id  )
    {
        if ( id <= 0 )
        {
            return BadRequest(new { message = "Invalid file ID" });
        }
        var currentUsernameClaim = User.FindFirst(JWTClaims.Name);
        if (currentUsernameClaim == null)
            return Unauthorized(new { message = "User not authenticated" });
        var userName = currentUsernameClaim.Value;
        try {
            await _fileService.DeleteFile(id, userName);
        } catch ( Exception ex ) {
            _logger.LogError(ex, "Error deleting file");
            return StatusCode(500, new { message = "An error occurred while deleting the file." });
        }
        return Ok(new { message = "File deleted successfully" });


    }



    [HttpPost("share/{id}")]
    [Authorize]
    public async Task<IActionResult> ShareFile(int id, [FromQuery] int duration)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid file ID" });
        var currentUsernameClaim = User.FindFirst(JWTClaims.Name);
        if (currentUsernameClaim == null)
            return Unauthorized(new { message = "User not authenticated" });
        var userName = currentUsernameClaim.Value;

        var shareToken = await _fileService.ShareFile(id, userName, duration);
        if (string.IsNullOrEmpty(shareToken))
            return BadRequest(new { message = "Failed to create share link" }); 


        string baseUrl;
        if (_environment.IsDevelopment())
        {
            baseUrl = _configuration["FrontendUrls:Development"] ?? "http://localhost:5173";
        }
        else
        {
            baseUrl = _configuration["FrontendUrls:Production"] ?? $"{Request.Scheme}://{Request.Host}";
        }

        
        string shareLink = $"{baseUrl}/download/shared/{shareToken}";

        return Ok(new { shareLink, duration });
    }

    [HttpGet("download/{id}")]
    [Authorize]
    public async Task<IActionResult> DownloadFile(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid file ID" });
        var currentUsernameClaim = User.FindFirst(JWTClaims.Name);
        if (currentUsernameClaim == null)
            return Unauthorized(new { message = "User not authenticated" });
        var userName = currentUsernameClaim.Value;

        var url = await _fileService.DonwloadFile(id, userName);
        
        return Ok(new { message = new { downloadurl = url } });  
    }


    [HttpGet("download/shared/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSharedFile(string token)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Invalid token" });

        try
        {
            var url = await _fileService.GetSharedFile(token);
            if (url == null)
                return NotFound(new { message = "File not found" });
                
            
            return Ok(new { downloadUrl = url });
            
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving shared file");
            return StatusCode(500, new { message = "An error occurred while retrieving the shared file." });
        }
    }
    
    [HttpGet("folder/{folderId}")]
    [Authorize]
    public async Task<IActionResult> GetFolderFiles(int folderId)
    {
        if (folderId <= 0)
            return BadRequest(new { message = "Invalid folder ID" });
        var currentUsernameClaim = User.FindFirst(JWTClaims.Name);
        if (currentUsernameClaim == null)
            return Unauthorized(new { message = "User not authenticated" });
        var userName = currentUsernameClaim.Value;

        try
        {
            var result = await _fileService.GetFolderFiles(folderId, userName);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving folder files");
            return StatusCode(500, new { message = "An error occurred while retrieving the folder files." });
        }
    }






}