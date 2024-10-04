using MessengerService.DTO;
using System.Text.RegularExpressions;

namespace MessengerService.Util.Validator
{
    public static class AuthValidator
    {
        private static (bool IsValid, string Message) ValidateCredentials(string email, string password)
        {
            var emailValidation = ValidateEmail(email);
            if (!emailValidation.IsValid)
                return emailValidation;

            var passwordValidation = ValidatePassword(password);
            if (!passwordValidation.IsValid)
                return passwordValidation;

            return (true, "Credentials are valid.");
        }

        public static (bool IsValid, string Message) ValidateNewUser(NewUserDTO newUser)
        {
            var credentialsValidation = ValidateCredentials(newUser.Email, newUser.Password);
            if (!credentialsValidation.IsValid)
                return credentialsValidation;

            var usernameValidation = ValidateUsername(newUser.Username);
            if (!usernameValidation.IsValid)
                return usernameValidation;

            return (true, "Validation successful.");
        }

        public static (bool IsValid, string Message) ValidateLogin(LoginUserDTO loginUser)
        {
            return ValidateCredentials(loginUser.Email, loginUser.Password);
        }

        private static (bool IsValid, string Message) ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email cannot be empty.");

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
                return (false, "Invalid email format.");

            return (true, "Email is valid.");
        }

        private static (bool IsValid, string Message) ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password cannot be empty.");
            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long.");

            return (true, "Password is valid.");
        }

        private static (bool IsValid, string Message) ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return (false, "Username cannot be empty.");

            return (true, "Username is valid.");
        }
    }
}