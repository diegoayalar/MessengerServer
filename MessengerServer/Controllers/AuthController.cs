using Firebase.Database;
using MessengerDomain.Entities;
using MessengerService.DTO;
using MessengerService.IServices;
using Microsoft.AspNetCore.Mvc;

namespace MessengerServer.Controllers
{
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService authService,
        ITokenService tokenService,
        IConfiguration config) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IConfiguration _config = config;

        [HttpPost("signup")]
        public async Task<IActionResult> RegisterUser([FromBody] NewUserDTO newUser)
        {
            var (success, Message) = await _authService.RegisterUserAsync(newUser);
            return success ? await GenerateTokenResponseAsync(newUser.Email) : BadRequest(Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUser)
        {
            var (success, response) = await _authService.LoginAsync(loginUser);
            return success ? await GenerateTokenResponseAsync(loginUser.Email) : BadRequest(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["AuthToken"];
            var (success, response) = await _authService.DeleteRefreshTokenAsync(refreshToken);

            if(!success)
                return Unauthorized(response);

            Response.Cookies.Delete("AuthToken");

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["AuthToken"];

            var (success, response) = await _authService.RefreshTokenAsync(refreshToken);

            if (!success)
                return Unauthorized(response);

            return await GenerateTokenResponseAsync(response);
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken()
        {
            try
            {
                if (!Request.Cookies.TryGetValue("AuthToken", out var token))
                    return BadRequest(new { Error = "No authentication token found." });

                var (isValid, message) = await _authService.ValidateTokenAsync(token);
                return isValid ? Ok("Token valid") : Unauthorized(new { Error = message });
            }
            catch (FirebaseAdmin.Auth.FirebaseAuthException ex)
            {
                return Unauthorized(new { Error = "Invalid or expired token.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An internal error occurred.", Details = ex.Message });
            }
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] LoginUserDTO userToDelete)
        {
            var (success, message) = await _authService.DeleteAccountAsync(userToDelete);
            await Logout();
            return success ? Ok("User account deleted successfully.") : BadRequest(message);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            await _authService.SendPasswordResetEmailAsync(email);
            return Ok("Password reset email sent.");
        }

        private async Task<IActionResult> GenerateTokenResponseAsync(string email)
        {
            var refreshToken = _tokenService.GenerateRefreshToken();
            var (success, Message) = await _authService.SaveRefreshTokenAsync(email, refreshToken);

            if (!success) return BadRequest(Message);

            var accessToken = _tokenService.GenerateAccessToken(email);

            SetRefreshTokenCookie(refreshToken.Token);
            return Ok(accessToken);
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(Convert.ToDouble(_config["JwtSettings:AccessTokenExpiration"]))
            };

            Response.Cookies.Append("AuthToken", refreshToken, cookieOptions);
        }
    }
}
