namespace FileUploader.Api.Infrastructure;
using Database;
using Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class AuthService {
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher _passwordHasher;
    private readonly TokenService _tokenService;
    private readonly ILogger<AuthService> _logger;
    private readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(7);
    public AuthService(ApplicationDbContext context, PasswordHasher passwordHasher, TokenService tokenService, ILogger<AuthService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }
    public async Task<UserRegister?> RegisterUserAsync(UserRegister UserReg)
    {
        if (await _context.Users.AnyAsync(x => x.Username == UserReg.Username))
        {
            _logger.LogWarning("Attempt to Register a user with duplicate username: {Username}", UserReg.Username);
            throw new InvalidOperationException("Username already exists");
        }

        var user = new User
        {
            Username = UserReg.Username,
            FirstName = UserReg.FirstName,
            LastName = UserReg.LastName,
            PasswordHash = _passwordHasher.Hash(UserReg.Password)

        };

        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            _logger.LogError(ex, "Error saving new user {Username} to the database", user.Username);
            throw new Exception("The username is already in use.");
        }

        return UserReg;
    }

    public async Task<Tokens> LoginUserAsync(UserLogin requestLogin)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == requestLogin.Username);
        if (user == null || !_passwordHasher.Verify(requestLogin.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for {Username}", requestLogin.Username);
            throw new InvalidOperationException("Invalid username or password");
        }

        Tokens tokens = _tokenService.Create(user);


        var refreshTokenExpiry =   DateTime.UtcNow.Add(_refreshTokenLifetime);
     
        
        await _context.RefreshTokens.AddAsync(new UserRefreshToken
        {
            UserName = user.Username,
            RefreshToken = tokens.RefreshToken,
            Expires = refreshTokenExpiry,
            IsActive = true
        });

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error saving refresh token to the database");
            throw new Exception("Error processing login. Please try again.");
        }

        return tokens;
    }
    
    public async Task<bool> UserNameCheckAsync(string username)
    {
        return await _context.Users.AnyAsync(x => x.Username == username);
    }

}