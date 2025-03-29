using System.ComponentModel.DataAnnotations;

namespace FileUploader.Api.Models;

public class User
{
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty; 
    
    
    [Required]
    [Key]
    public string Username 
    { 
        get => _username;
        set => _username = value?.Trim() ?? string.Empty;
    }
    private string _username = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty; 
    
    
}