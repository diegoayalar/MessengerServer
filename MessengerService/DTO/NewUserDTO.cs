namespace MessengerDomain.DTOs
{
    public class NewUserDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Username { get; set; }
    }
}
