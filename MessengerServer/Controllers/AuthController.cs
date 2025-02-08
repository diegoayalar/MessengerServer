using MessengerService.DTO;
using MessengerService.IServices;
using Microsoft.AspNetCore.Mvc;

namespace MessengerServer.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> RegisterUser([FromBody] NewUserDTO newUser)
        {
            var (success, response) = await _authService.RegisterUserAsync(newUser);

            if (!success)
            {
                return BadRequest(response);
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,  // Prevents client-side access
                Secure = false,    // Requires HTTPS
                SameSite = SameSiteMode.Strict, // Prevents CSRF attacks
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("AuthToken", response, cookieOptions);

            return Ok("Register successful.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserDTO loginUser)
        {
            var (success, response) = await _authService.LoginAsync(loginUser);

            if (!success)
            {
                return BadRequest(response);
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("AuthToken", response, cookieOptions);

            return Ok("Login successful.");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _authService.SignOutUser();
            return Ok("User signed out successfully.");
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            var (isValid, userIdOrMessage) = await _authService.ValidateTokenAsync(token);

            if (!isValid)
            {
                return BadRequest(new { Error = userIdOrMessage });
            }

            return Ok(new { UserId = userIdOrMessage });
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] LoginUserDTO userToDelete)
        {
            var (success, message) = await _authService.DeleteAccountAsync(userToDelete);

            if (!success)
            {
                return BadRequest(message);
            }

            return Ok("User account deleted successfully.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            await _authService.SendPasswordResetEmailAsync(email);
            return Ok("Password reset email sent.");
        }
    }
}
