namespace MessengerDomain.Entities
{
    public class User
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public ICollection<string>? Chats { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool IsActive { get; set; } = true;
        public RefreshToken? RefreshToken { get; set; }
        public Profile? Profile { get; set; }

    }
}
