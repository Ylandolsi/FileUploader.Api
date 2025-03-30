using System.ComponentModel.DataAnnotations;

namespace FileUploader.Api.Models;

public class SharedFile{
    [Key]
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string SharedBy { get; set; }

    public string Url { get; set; }

}