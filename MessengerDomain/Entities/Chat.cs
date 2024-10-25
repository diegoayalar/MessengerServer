namespace MessengerDomain.Entities
{
    public class Chat
    {
        public string? Id { get; set; }
        public Dictionary<string, Message>? Messages { get; set; }

        public required ICollection<string>? Users { get; set; }

        public string? ChatName { get; set; } = "New chat";

        public string? Description { get; set; } = string.Empty;

        public string? GroupPic { get; set; } = string.Empty;

        public bool IsGroup { get; set; } = false;

        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}
