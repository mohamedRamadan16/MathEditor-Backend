namespace Api.Application.Documents.DTOs;

public class DocumentCreateDto
{
    public string Handle { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Published { get; set; }
    public bool Collab { get; set; }
    public bool Private { get; set; }
    public List<string> Coauthors { get; set; } = new();
    public InitialRevisionDto InitialRevision { get; set; } = new();
}

public class InitialRevisionDto
{
    public string Data { get; set; } = string.Empty;
}
