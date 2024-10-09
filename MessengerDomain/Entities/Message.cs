namespace MessengerDomain.Entities
{
    public class Message
    {
        public string? _Id { get; set; }

        public string? MessageText { get; set; } = string.Empty;

        public required string? Sender { get; set; } = string.Empty;

        public string? File { get; set; } = string.Empty;

        public bool ReadState { get; set; } = false;

        public ICollection<string>? RecivedUsers { get; set; }

        public ICollection<string>? UnrecivedUsers { get; set; }

        public ICollection<string>? ReadUsers { get; set; }

        public ICollection<string>? UneadUsers { get; set; }

        public DateTime? DateSent { get; set; } = DateTime.Now;
    }
}
