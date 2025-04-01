namespace FileUploader.Api.Infrastructure;
using Database;
using Dtos;
using Models;
using Microsoft.EntityFrameworkCore;


public class RefreshService
{
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RefreshService> _logger;
        private readonly TokenService _tokenService;
        private readonly TimeSpan _refreshTokenLifetime = TimeSpan.FromDays(7); 

        public RefreshService(
            ApplicationDbContext context,
            ILogger<RefreshService> logger,
            TokenService tokenService)
        {
            _context = context;
            _logger = logger;
            _tokenService = tokenService;
        }


        public async Task<Tokens> ValidateAndRotateTokenAsync(RefreshTokenDto refreshTokenRequest)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var existingToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => 
                        rt.UserName == refreshTokenRequest.UserName && 
                        rt.RefreshToken == refreshTokenRequest.RefreshToken);

                if (existingToken == null)
                {
                    throw new InvalidOperationException("Refresh token not found.");
                }

                if (!existingToken.IsActive)
                {
                    throw new InvalidOperationException("Refresh token has been revoked.");
                }

                if (existingToken.Expires < DateTime.UtcNow)
                {
                    throw new InvalidOperationException("Refresh token has expired.");
                }


                _context.RefreshTokens.Remove(existingToken);


                var tokens = _tokenService.Create(refreshTokenRequest.UserName);

                await _context.RefreshTokens.AddAsync(new UserRefreshToken
                {
                    UserName = refreshTokenRequest.UserName,
                    RefreshToken = tokens.RefreshToken,
                    Expires = DateTime.UtcNow.Add(_refreshTokenLifetime),
                    IsActive = true
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return tokens;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    
        
}