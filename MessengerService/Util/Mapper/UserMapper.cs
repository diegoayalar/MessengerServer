using MessengerDomain.Entities;
using MessengerService.DTO;

public static class UserMapper
{
    private static User MapCommonFields(string email, string password)
    {
        return new User
        {
            Email = email,
            Password = password,
            DateCreated = DateTime.UtcNow
        };
    }

    public static User NewUserToUser(NewUserDTO newUser)
    {
        var user = MapCommonFields(newUser.Email, newUser.Password);
        user.Profile = new Profile { Name = newUser.Username };
        return user;
    }

    public static User LoginUserToUser(LoginUserDTO loginUser)
    {
        return MapCommonFields(loginUser.Email, loginUser.Password);
    }
}