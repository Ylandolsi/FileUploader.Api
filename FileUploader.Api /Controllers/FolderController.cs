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
public class FolderController : ControllerBase
{
    private readonly FolderService _folderService;
    private readonly ILogger<FolderController> _logger;

    public FolderController(FolderService folderService, ILogger<FolderController> logger)
    {
        _folderService = folderService;
        _logger = logger;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateFolder ([FromBody] CreateFolderDto createFolderDto)
    {
        if (string.IsNullOrEmpty(createFolderDto.Name))
            return BadRequest(new { message = "Folder name is required" });


        var currentUsernameClaim = User.FindFirst(JWTClaims.Name); 
        if (currentUsernameClaim == null)
            return Unauthorized(new { message = "User not authenticated" });
        var userName = currentUsernameClaim.Value;  

        try
        {
            var result = await _folderService.CreateFolder(createFolderDto, userName);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteFolder(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid folder ID" });
        if ( id == 1 )
            return BadRequest(new { message = "Root folder cannot be deleted" });

        var currentUsernameClaim = User.FindFirst(JWTClaims.Name); 
        if (currentUsernameClaim == null)
            return Unauthorized(new { message = "User not authenticated" });
        var userName = currentUsernameClaim.Value;  
        try
        {
            await _folderService.DeleteFolder(id, userName);
            return Ok(new { message = "Folder deleted successfully" });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetFolderContents(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid folder ID" });

        var currentUsernameClaim = User.FindFirst(JWTClaims.Name); 
        if (currentUsernameClaim == null)
            return Unauthorized(new { message = "User not authenticated" });
        var userName = currentUsernameClaim.Value;  

        try
        {
            var result = await _folderService.GetFolderContents(id, userName);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}
