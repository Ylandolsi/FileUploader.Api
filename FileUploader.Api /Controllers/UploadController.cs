using FileUploader.Api.Dtos;
using FileUploader.Api.Exceptions;
using FileUploader.Api.Models;
using FileUploader.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Api.Controllers;


using JWTClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using InvalidOperationException = Exceptions.InvalidOperationException;
[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase{

    private readonly UploadService _uploadService;
    private readonly ILogger<UploadController> _logger;

    public UploadController(UploadService uploadService, ILogger<UploadController> logger) {
        _uploadService = uploadService;
        _logger = logger;
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
            var result = await _uploadService.UploadFile(file, folderId, userName);
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




}