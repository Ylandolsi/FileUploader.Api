using System.ComponentModel.DataAnnotations;

namespace FileUploader.Api.Models;

public class User
{
    [Required]
    [Key]
    public string Username 
    { 
        get => _username;
        set => _username = value?.Trim() ?? string.Empty;
    }
    private string _username = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty; 
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty; 


    public ICollection<Folder> Folders { get; set; }
    public ICollection<FileItem> Files { get; set; }
    
    
}