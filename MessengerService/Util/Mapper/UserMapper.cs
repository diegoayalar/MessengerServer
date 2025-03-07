using MessengerDomain.Entities;
using MessengerService.DTO;

namespace MessengerService.Util.Mapper
{
    public static class UserMapper
    {
        private static User MapCommonFields(string email)
        {
            return new User
            {
                Email = email,
                DateCreated = DateTime.UtcNow
            };
        }

        public static User NewUserToUser(NewUserDTO newUser)
        {
            var user = MapCommonFields(newUser.Email);
            user.Profile = new Profile
            {
                Name = newUser.Username,
                Status = 0,
            };
            return user;
        }
    }
}