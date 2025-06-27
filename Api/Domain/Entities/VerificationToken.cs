namespace Api.Domain.Entities
{
    public class VerificationToken
    {
        public int Id { get; set; }
        public string Identifier { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
    }
}
