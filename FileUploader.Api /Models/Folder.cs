namespace FileUploader.Api.Models;
public class Folder
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentFolderId { get; set; }
    public Folder ParentFolder { get; set; }
    public ICollection<Folder> ChildFolders { get; set; }
    public ICollection<FileItem> Files { get; set; }


    public string Username { get; set; } 
    public User User { get; set; } 
}