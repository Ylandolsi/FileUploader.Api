using FileUploader.Api.Dtos;
using FileUploader.Api.Infrastructure;
using FileUploader.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Api.Controllers;


using JWTClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RefreshService _refreshService;
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(RefreshService refreshService,
        ILogger<AuthController> logger ,
        AuthService authService)
    {
        _refreshService = refreshService;
        _authService = authService;
        _logger = logger;
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshTokenRequest)
    {
        try
        {
            var refreshToken = await _refreshService.ValidateAndRotateTokenAsync(refreshTokenRequest);
            return Ok(refreshToken);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid refresh token: {Message}", ex.Message);
            return Unauthorized(new { message = "Invalid or expired refresh token." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, new { message = "An unexpected error occurred while refreshing the token." });
        }
    }


    
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(UserRegister UserReg)
    {
        try
        {
            var registeredUser = await _authService.RegisterUserAsync(UserReg);
            return Ok(registeredUser);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid registration attempt: {Message}", ex.Message);
            return BadRequest(new { message = "Registration failed. Username may already exist." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return StatusCode(500, new { message = "An unexpected error occurred during registration." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLogin user)
    {
        try
        {
            var loggedInUser = await _authService.LoginUserAsync(user);
            return Ok(loggedInUser);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Login failed: {Message}", ex.Message);
            return Unauthorized(new { message = "Invalid username or password." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user");
            return StatusCode(500, new { message = "An unexpected error occurred during login." });
        }
    }

    
    [HttpGet("check-username")]
    
    public async Task<IActionResult> UserNameCheck([FromQuery]string username)
    {
        if( username == null)
        {
            return BadRequest();
        }
        var isAvaialble = await _authService.UserNameCheckAsync(username);
        return Ok(new {available = !isAvaialble});
    }
}