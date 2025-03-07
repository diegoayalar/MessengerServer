using Firebase.Auth;
using MessengerDomain.Entities;
using MessengerService.DTO;
using MessengerService.IServices;
using MessengerService.Util.Mapper;
using MessengerService.Util.Validator;

namespace MessengerService.Services
{
    public class AuthService(IUserService userService,
        IFirebaseAuthClient firebaseAuthClient) : IAuthService
    {
        private readonly IUserService _userService = userService;
        private readonly IFirebaseAuthClient _firebaseAuthClient = firebaseAuthClient;

        public async Task<(bool Success, string? Message)> RegisterUserAsync(NewUserDTO newUser)
        {
            var (IsValid, Message) = AuthValidator.ValidateNewUser(newUser);
            if (!IsValid)
                return (false, Message);

            try
            {
                await RegisterFirebaseUserAsync(newUser);

                return (true, null);
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
                return (false, "Invalid email or password.");

            var token = await userCredentials.User.GetIdTokenAsync();

            return (true, token);
        }

        public void SignOutUser()
        {
            _firebaseAuthClient.SignOut();
        }

        public async Task<(bool Success, string? Message)> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return (false, "Refresh token is missing.");

            var user = await _userService.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null)
                return (false, "User not found.");

            if (user.RefreshToken == null)
                return (false, "Refresh token is missing.");

            if (user.RefreshToken.Expiry <= DateTime.Now)
                return (false, "Refresh token has expired.");

            return (true, user.Email);
        }

        public async Task<(bool Success, string? Message)> SaveRefreshTokenAsync(string email, RefreshToken refreshToken)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
                return (false, "User not found");

            await _userService.UpdateUserFieldAsync(
                user.Id, user =>user.RefreshToken = refreshToken);

            return (true, null);
        }

        public async Task<(bool Success, string Message)> DeleteRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return (false, "Refresh token is missing.");

            var user = await _userService.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null)
                return (false, "User not found");

            await _userService.UpdateUserFieldAsync(
                user.Id, user => user.RefreshToken = null);

            return (true, "User signed out successfully.");
        }

        public async Task<(bool IsValid, string? Message)> ValidateTokenAsync(string token)
        {
            try
            {
                var decodedToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                return (true, null);
            }
            catch (FirebaseAuthException ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> DeleteAccountAsync(LoginUserDTO userToDelete)
        {
            var user = await _userService.GetUserByEmailAsync(userToDelete.Email);
            if (user == null) 
                return (false, "User not found.");

            var userCredentials = await SignInWithEmailAndPasswordAsync(user.Email, userToDelete.Password);
            if (userCredentials == null) 
                return (false, "Invalid email or password.");

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

        private async Task RegisterFirebaseUserAsync(NewUserDTO newUser)
        {
            var userCredentials = await _firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(newUser.Email, newUser.Password);

            await AddNewUserToDBAsync(newUser, userCredentials.User.Uid);
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

        private async Task AddNewUserToDBAsync(NewUserDTO newUser, string userId)
        {
            var user = UserMapper.NewUserToUser(newUser);
            user.Id = userId;

            var insertedUser = await _userService.InsertUserAsync(user);
        }
    }
}
