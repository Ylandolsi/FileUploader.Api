using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileUploader.Api.Models;

public class UserRefreshToken
{
    [Key]
    public Guid Id { get; set; }
        
    [Required]
    public string UserName{ get; set; }
        
    [Required]

    public string RefreshToken { get; set; } = string.Empty;
        
    public DateTime Expires { get; set; }
        
    public bool IsActive { get; set; } = true;

        
    [ForeignKey(nameof(UserName))]
    public virtual User? User { get; set; }
}