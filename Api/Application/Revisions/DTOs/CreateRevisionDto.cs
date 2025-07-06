using System.Text.Json;

namespace Api.Application.Revisions.DTOs
{
    public class CreateRevisionDto
    {
        public Guid DocumentId { get; set; }
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
}