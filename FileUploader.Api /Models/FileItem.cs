namespace FileUploader.Api.Models;

public class FileItem
{

    public int Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public int Size { get; set; }
    public DateTime CreatedAt { get; set; }

    public int FolderId { get; set; }
    public Folder Folder { get; set; }


    public string Username { get; set; } 
    public User User { get; set; } 
}
