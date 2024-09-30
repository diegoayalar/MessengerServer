namespace MessengerDomain.Entities
{
    public class Message
    {
        public string? MessageText { get; set; }

        public string? File {  get; set; }

        public bool ReadState { get; set; }

        public ICollection<string>? RecivedUsers { get; set; }

        public ICollection<string>? UnrecivedUsers { get; set; }

        public ICollection<string>? ReadUsers { get; set; }

        public ICollection<string>? UneadUsers { get; set; }

        public DateTime? DateSent { get; set; } = DateTime.Now;
    }
}
