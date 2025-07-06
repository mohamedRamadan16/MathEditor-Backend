using System.Text.Json.Serialization;

namespace Api.Application.Revisions.DTOs
{
    public class LexicalStateDto
    {
        [JsonPropertyName("root")]
        public LexicalRootDto Root { get; set; } = null!;
    }

    public class LexicalRootDto
    {
        [JsonPropertyName("children")]
        public List<LexicalNodeDto> Children { get; set; } = new();
        
        [JsonPropertyName("direction")]
        public string? Direction { get; set; }
        
        [JsonPropertyName("format")]
        public string Format { get; set; } = string.Empty;
        
        [JsonPropertyName("indent")]
        public int Indent { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; } = "root";
        
        [JsonPropertyName("version")]
        public int Version { get; set; }
    }

    public class LexicalNodeDto
    {
        [JsonPropertyName("children")]
        public List<LexicalNodeDto>? Children { get; set; }
        
        [JsonPropertyName("direction")]
        public string? Direction { get; set; }
        
        [JsonPropertyName("format")]
        public string Format { get; set; } = string.Empty;
        
        [JsonPropertyName("indent")]
        public int Indent { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        
        [JsonPropertyName("version")]
        public int Version { get; set; }
        
        // Text node specific properties
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        
        [JsonPropertyName("detail")]
        public int? Detail { get; set; }
        
        [JsonPropertyName("mode")]
        public string? Mode { get; set; }
        
        [JsonPropertyName("style")]
        public string? Style { get; set; }
        
        // Math node specific properties
        [JsonPropertyName("value")]
        public string? Value { get; set; }
        
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        
        // Image/Sketch/Graph node specific properties
        [JsonPropertyName("src")]
        public string? Src { get; set; }
        
        [JsonPropertyName("altText")]
        public string? AltText { get; set; }
        
        [JsonPropertyName("width")]
        public int? Width { get; set; }
        
        [JsonPropertyName("height")]
        public int? Height { get; set; }
        
        [JsonPropertyName("showCaption")]
        public bool? ShowCaption { get; set; }
        
        [JsonPropertyName("caption")]
        public object? Caption { get; set; }
        
        // Additional properties that might be used by various node types
        [JsonPropertyName("key")]
        public string? Key { get; set; }
    }
}
