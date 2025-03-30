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
    private readonly TokenService _tokenService;
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(RefreshService refreshService,
        ILogger<AuthController> logger , TokenService tokenService,
        AuthService authService)
    {
        _refreshService = refreshService;
        _tokenService = tokenService;
        _authService = authService;
        _logger = logger;
    }
    
    [HttpPost("refresh-token")]
    [Authorize]
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


    [HttpGet("validate")]
    public IActionResult ValidateJWT()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
    
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return Unauthorized(new { valid = false, message = "Authorization header missing or invalid." });
    
        var token = authHeader.Substring(7);
    
        try
        {
            var principal = _tokenService.validate(token);
            if (principal == null)
                return Unauthorized(new { valid = false, message = "Invalid token." });
            
            //var claims = principal.Claims.Select(c => new { c.Type, c.Value });
            var username = principal.FindFirst(JWTClaims.Name)?.Value;
            //return Ok(new { valid = true, claims });
            
            return Ok(new { valid = true, username });
        }
        catch (Exception)
        {
            return Unauthorized(new { valid = false, message = "Invalid token." });
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




    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenDto refreshTokenRequest)
    {
        try
        {
            _logger.LogInformation("Logging out user");
            var result = await _refreshService.DeleteRefreshTokenAsync(refreshTokenRequest);
            
            if (!result)
            {
                _logger.LogWarning("Token not found during logout");
                return NotFound(new { message = "Token not found." });
            }
            
            _logger.LogInformation("User logged out");
            return Ok(new { message = "Logout successful. Client should dispose of all tokens." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging out user");
            return StatusCode(500, new { message = "An unexpected error occurred during logout." });
        }
    }
}