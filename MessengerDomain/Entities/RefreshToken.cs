namespace MessengerDomain.Entities
{
    public class RefreshToken
    {
        public required string Token { get; set; }
        public required DateTime Expiry { get; set; }
    }
}
