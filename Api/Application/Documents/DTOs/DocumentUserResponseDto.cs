namespace Api.Application.Documents.DTOs;

public class DocumentUserResponseDto
{
    public Guid Id { get; set; }
    public string? Handle { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Image { get; set; }
}
