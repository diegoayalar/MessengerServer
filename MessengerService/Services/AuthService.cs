using Firebase.Auth;
using MessengerService.DTO;
using MessengerService.IServices;
using MessengerService.Util;
using MessengerService.Util.Validator;

namespace MessengerService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IFirebaseAuthClient _firebaseAuthClient;

        public AuthService(IUserService userService, IFirebaseAuthClient firebaseAuthClient)
        {
            _userService = userService;
            _firebaseAuthClient = firebaseAuthClient;
        }

        public async Task<(bool Success, string? Token)> RegisterUserAsync(NewUserDTO newUser)
        {
            var validation = AuthValidator.ValidateNewUser(newUser);
            if (!validation.IsValid) return (false, validation.Message);

            try
            {
                var token = await RegisterFirebaseUserAsync(newUser);

                return (true, token);
            }
            catch (FirebaseAuthHttpException ex) when (ex.Reason == AuthErrorReason.EmailExists)
            {
                return (false, "The email is already registered. Please use a different email.");
            }
            catch (FirebaseAuthHttpException ex)
            {
                return (false, $"Firebase error: {ex.Reason}");
            }
            catch (Exception ex)
            {
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Token)> LoginAsync(LoginUserDTO loginUser)
        {
            var userCredentials = await SignInWithEmailAndPasswordAsync(loginUser.Email, loginUser.Password);
            if (userCredentials == null)
            {
                return (false, "Invalid email or password.");
            }

            var token = await userCredentials.User.GetIdTokenAsync();
            return (true, token);
        }

        public void SignOutUser()
        {
            _firebaseAuthClient.SignOut();
        }

        public async Task<(bool IsValid, string UserId)> ValidateTokenAsync(string token)
        {
            try
            {
                var decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                return (true, decodedToken.Uid);
            }
            catch (FirebaseAuthException ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> DeleteAccountAsync(LoginUserDTO userToDelete)
        {
            var user = await _userService.GetUserByEmailAsync(userToDelete.Email);
            if (user == null) return (false, "User not found.");

            var userCredentials = await SignInWithEmailAndPasswordAsync(user.Email, userToDelete.Password);
            if (userCredentials == null) return (false, "Invalid email or password.");

            await userCredentials.User.DeleteAsync();
            await _userService.UpdateUserFieldAsync(user.Id, u => u.IsActive = false);
            await _userService.DeleteUserDataAsync(user);
            SignOutUser();
            return (true, null);
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            await _firebaseAuthClient.ResetEmailPasswordAsync(email);
        }

        private async Task<string> RegisterFirebaseUserAsync(NewUserDTO newUser)
        {
            var userCredentials = await _firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(newUser.Email, newUser.Password);
            await AddNewUserToDBAsync(newUser, userCredentials.User.Uid);
            return await userCredentials.User.GetIdTokenAsync();
        }

        private async Task<UserCredential?> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            try
            {
                return await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch (FirebaseAuthException)
            {
                return null;
            }
        }

        private async Task<string?> CheckIfUserExistsAsync(string email)
        {
            var existingUser = await _userService.GetUserByEmailAsync(email);
            return existingUser != null ? $"A user with email {existingUser.Email} already exists." : null;
        }

        private async Task AddNewUserToDBAsync(NewUserDTO newUser, string userId)
        {
            var hashedPassword = PasswordHelper.HashPassword(newUser.Password);
            newUser.Password = hashedPassword;

            var user = UserMapper.NewUserToUser(newUser);
            user.Id = userId;
            await _userService.InsertUserAsync(user);
        }
    }
}
