namespace MessengerDomain.Entities
{
    public class Profile
    {
        public required string Name {  get; set; }

        public string? Description { get; set; }

        public string? ProfilePic {  get; set; }

        public int Status {  get; set; }    

    }
}
