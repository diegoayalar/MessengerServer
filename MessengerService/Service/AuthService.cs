using MessengerService.DTO;
using MessengerService.Util;
using MessengerService.Util.Validator;

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

            var hashedPassword = PasswordHelper.HashPassword(newUser.Password);
            newUser.Password = hashedPassword;

            var user = UserMapper.NewUserToUser(newUser);
            await _userService.InsertUserAsync(user);

            return "User registered successfully.";

        }

        public async Task<string> Login(LoginUserDTO loginUser)
        {
            var validation = AuthValidator.ValidateLogin(loginUser);
            if (!validation.IsValid)
            {
                return validation.Message;
            }

            var existingUser = await _userService.GetUserByEmailAsync(loginUser.Email);
            if (existingUser == null)
            {
                return $"There is no user with email {loginUser.Email}";
            }

            if (!PasswordHelper.VerifyPassword(loginUser.Password, existingUser.Password))
            {
                return "Incorrect password.";
            }

            return "Login successful.";
        }
    }
}