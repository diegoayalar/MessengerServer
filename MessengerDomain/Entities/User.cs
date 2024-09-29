namespace MessengerDomain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string EmailAddress { get; set; }
        public required string Password { get; set; }
        public required string PhoneNumber { get; set; }
        public required Profile Profile { get; set; }
    }
}
