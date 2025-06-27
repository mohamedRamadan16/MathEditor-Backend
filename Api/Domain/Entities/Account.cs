namespace Api.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } = null!;
        public string Provider { get; set; } = null!;
        public string ProviderAccountId { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public string? AccessToken { get; set; }
        public int? ExpiresAt { get; set; }
        public string? TokenType { get; set; }
        public string? Scope { get; set; }
        public string? IdToken { get; set; }
        public string? SessionState { get; set; }
        public string? OAuthTokenSecret { get; set; }
        public string? OAuthToken { get; set; }
        public User User { get; set; } = null!;
    }
}
