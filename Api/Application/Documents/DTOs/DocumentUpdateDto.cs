namespace Api.Application.Documents.DTOs;

public class DocumentUpdateDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Handle { get; set; }
    public bool? Published { get; set; }
    public bool? Collab { get; set; }
    public bool? Private { get; set; }
}
