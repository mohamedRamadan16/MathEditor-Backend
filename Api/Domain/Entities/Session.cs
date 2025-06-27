namespace Api.Domain.Entities
{
    public class Session
    {
        public Guid Id { get; set; }
        public string SessionToken { get; set; } = null!;
        public Guid UserId { get; set; }
        public DateTime Expires { get; set; }
        public User User { get; set; } = null!;
    }
}
