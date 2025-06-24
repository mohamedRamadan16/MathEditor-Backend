namespace Api.Application.Users.DTOs
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string? Handle { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Image { get; set; }
    }
}
