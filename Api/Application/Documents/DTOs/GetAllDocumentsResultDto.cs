namespace Api.Application.Documents.DTOs;

public class GetAllDocumentsResultDto
{
    public IEnumerable<DocumentResponseDto> Items { get; set; } = null!;
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
