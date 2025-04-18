namespace FileUploader.Api.Dtos;
public class FileItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Size { get; set; }
    public string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public int FolderId { get; set; }
}