using Api.Application.Revisions.DTOs;
using System.Text.Json;

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
    public LexicalStateDto Data { get; set; } = null!;
    
    /// <summary>
    /// Converts the structured Data to JSON string for storage
    /// </summary>
    public string GetDataAsJson()
    {
        return JsonSerializer.Serialize(Data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}
