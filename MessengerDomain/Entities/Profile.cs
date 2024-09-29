namespace MessengerDomain.Entities
{
    public class Profile
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required int Status { get; set; }
        public int ProfilePicURL { get; set; }
    }
}
