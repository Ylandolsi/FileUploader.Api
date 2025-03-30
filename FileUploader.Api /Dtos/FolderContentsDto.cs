namespace FileUploader.Api.Dtos;
public class FolderContentsDto
{
    public List<FolderDto> Subfolders { get; set; } = new();
    public List<FileItemDto> Files { get; set; } = new();
    public FolderDto CurrentFolder { get; set; } = new();
}