namespace Api.Application.Documents.DTOs;

public class DocumentUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Handle { get; set; } = string.Empty;
    public bool Published { get; set; }
    public bool Collab { get; set; }
    public bool Private { get; set; }
}
