using Firebase.Auth;
using MessengerService.DTO;
using MessengerService.Util;
using MessengerService.Util.Validator;

namespace MessengerService.Service
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly FirebaseAuthClient _firebaseAuthClient;

        public AuthService(UserService userService, FirebaseAuthClient firebaseAuthClient)
        {
            _userService = userService;
            _firebaseAuthClient = firebaseAuthClient;
        }

        public async Task<string> RegisterUserAsync(NewUserDTO newUser)
        {
            var validation = AuthValidator.ValidateNewUser(newUser);
            if (!validation.IsValid) return validation.Message;

            var userExistenceMessage = await CheckIfUserExistsAsync(newUser.Email);
            if (userExistenceMessage != null) return userExistenceMessage;

            return await RegisterFirebaseUserAsync(newUser);
        }

        public async Task<string?> LoginAsync(LoginUserDTO loginUser)
        {
            try
            {
                var userCredentials = await SignInWithEmailAndPasswordAsync(loginUser);
                return await userCredentials.User.GetIdTokenAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SignOutUser()
        {
            _firebaseAuthClient.SignOut();
        }

        public async Task<string?> DeleteAccountAsync(LoginUserDTO deleteUser)
        {
            try
            {
                var userCredentials = await SignInWithEmailAndPasswordAsync(deleteUser);
                await userCredentials.User.DeleteAsync();

                await _userService.UpdateUserIsActiveAsync(deleteUser.Email, false);

                return null;
            }
            catch (Exception)
            {
                return "User not found or invalid credentials.";
            }
        }

        public async Task<string?> SendPasswordResetEmailAsync(string email)
        {
            try
            {
                await _firebaseAuthClient.ResetEmailPasswordAsync(email);
                return null;
            }
            catch (Exception ex)
            {
                return $"Error sending password reset email: {ex.Message}";
            }
        }

        private async Task<string> RegisterFirebaseUserAsync(NewUserDTO newUser)
        {
            try
            {
                var userCredentials = await _firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(newUser.Email, newUser.Password);
                await AddNewUserToDBAsync(newUser);
                return await userCredentials.User.GetIdTokenAsync();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private async Task<UserCredential> SignInWithEmailAndPasswordAsync(LoginUserDTO loginUser)
        {
            var userCredentials = await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(loginUser.Email, loginUser.Password);
            return userCredentials;
        }

        private async Task<string?> CheckIfUserExistsAsync(string email)
        {
            var existingUser = await _userService.GetUserByEmailAsync(email);
            return existingUser != null ? $"A user with email {existingUser.Email} already exists." : null;
        }

        private async Task AddNewUserToDBAsync(NewUserDTO newUser)
        {
            var hashedPassword = PasswordHelper.HashPassword(newUser.Password);
            newUser.Password = hashedPassword;

            var user = UserMapper.NewUserToUser(newUser);
            await _userService.InsertUserAsync(user);
        }
    }
}