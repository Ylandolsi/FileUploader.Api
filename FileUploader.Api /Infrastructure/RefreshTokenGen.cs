namespace FileUploader.Api.Infrastructure;
using System.Security.Cryptography;

public static class RefreshTokenGen
{
    public static string Generate()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}