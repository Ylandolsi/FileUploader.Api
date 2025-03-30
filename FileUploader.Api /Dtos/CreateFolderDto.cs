namespace FileUploader.Api.Dtos;
public class CreateFolderDto
{
    public string Name { get; set; }
    public int? ParentFolderId { get; set; }

    
}