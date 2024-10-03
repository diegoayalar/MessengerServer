using MessengerDomain.DTOs;
using MessengerService.Mapper;
using MessengerService.Validator;

namespace MessengerService.Service
{
    public class AuthService
    {
        private readonly UserService _userService;

        public AuthService(UserService userService)
        {
            _userService = userService;
        }

        public async Task<string> RegisterUser(NewUserDTO newUser)
        {
            var validation = AuthValidator.ValidateNewUser(newUser);
            if (!validation.IsValid)
            {
                return validation.Message;
            }

            var existingUser = await _userService.GetUserByEmailAsync(newUser.Email);
            if (existingUser != null)
            {
                return $"A user with email {existingUser.Email} already exists.";
            }

            var hashedPassword = HashPassword(newUser.Password);
            newUser.Password = hashedPassword;

            var user = UserMapper.NewUserToUser(newUser);
            await _userService.InsertUserAsync(user);

            return "User registered successfully.";
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}