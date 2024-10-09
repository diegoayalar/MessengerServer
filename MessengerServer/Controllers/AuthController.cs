﻿using MessengerService.DTO;
using MessengerService.Service;
using Microsoft.AspNetCore.Mvc;

namespace MessengerServer.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] NewUserDTO newUser)
        {
            var tokenOrError = await _authService.RegisterUserAsync(newUser);

            if (tokenOrError.StartsWith("Error:") || tokenOrError.Contains("already exists"))
            {
                return BadRequest(new { Message = tokenOrError });
            }

            return Ok(tokenOrError);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserDTO loginUser)
        {
            var token = await _authService.LoginAsync(loginUser);

            if (token == null)
            {
                return BadRequest("Invalid credentials.");
            }

            return Ok(token);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _authService.SignOutUser();
            return Ok(new { Message = "User signed out successfully." });
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] LoginUserDTO deleteUser)
        {
            var result = await _authService.DeleteAccountAsync(deleteUser);

            if (result == null)
            {
                return Ok(new { Message = "User account deleted successfully." });
            }

            return BadRequest(new { Message = result });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            var result = await _authService.SendPasswordResetEmailAsync(email);

            if (result == null)
            {
                return Ok(new { Message = "Password reset email sent." });
            }

            return BadRequest(new { Message = result });
        }
    }
}
