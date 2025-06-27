namespace Api.Application.Users.DTOs
{
    public class UpdateUserDto
    {
        public string? Handle { get; set; }
        public string Name { get; set; } = string.Empty;
        //public string Email { get; set; } = string.Empty;
        public string? Image { get; set; }
    }
}
