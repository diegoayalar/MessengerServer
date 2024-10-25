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

        public async Task<(bool Success, string? Token)> RegisterUserAsync(NewUserDTO newUser)
        {
            var validation = AuthValidator.ValidateNewUser(newUser);
            if (!validation.IsValid) return (false, validation.Message);

            var userExistenceMessage = await CheckIfUserExistsAsync(newUser.Email);
            if (userExistenceMessage != null) return (false, userExistenceMessage);

            return (true, await RegisterFirebaseUserAsync(newUser));
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
            await AddNewUserToDBAsync(newUser);
            return await userCredentials.User.GetIdTokenAsync();
        }

        private async Task<UserCredential?> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            try
            {
                return await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch(FirebaseAuthException)
            {
                return null;
            }
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
            var insertedUser = await _userService.InsertUserAsync(user);
            user.Id = insertedUser.Key;
            await _userService.UpdateUserAsync(user);
        }
    }
}
