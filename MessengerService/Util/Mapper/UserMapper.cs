using MessengerDomain.DTOs;
using MessengerDomain.Entities;

namespace MessengerService.Util.Mapper
{
    public static class UserMapper
    {
        public static User NewUserToUser(NewUserDTO newUser)
        {
            return new User
            {
                Email = newUser.Email,
                Password = newUser.Password,
                Profile = new Profile
                {
                    Name = newUser.Username
                },
                DateCreated = DateTime.UtcNow
            };
        }
    }
}
