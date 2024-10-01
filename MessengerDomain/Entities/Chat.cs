namespace MessengerDomain.Entities
{
    public class Chat
    {
        public ICollection<Message>? Messages { get; set; } = null;

        public required ICollection<string> Users { get; set; }

        public string ChatName { get; set; } = "New chat";

        public string? Description { get; set; } = null;

        public string? GroupPic { get; set; } = null;

        public bool IsGroup { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}
