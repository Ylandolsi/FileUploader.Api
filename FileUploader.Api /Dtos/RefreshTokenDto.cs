using System.ComponentModel.DataAnnotations;

namespace FileUploader.Api.Dtos;

public class RefreshTokenDto
{
    
    [Required]
    public string UserName{ get; set; }
        
    [Required]

    public string RefreshToken { get; set; } = string.Empty;

}