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
            var resultMessage = await _authService.RegisterUser(newUser);

            if (resultMessage == "User registered successfully.")
            {
                return Ok(new { Message = resultMessage });
            }

            return BadRequest(new { Message = resultMessage });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUser)
        {
            var resultMessage = await _authService.Login(loginUser);

            if (resultMessage == "Login successful.")
            {
                return Ok(new { Message = resultMessage });
            }

            return BadRequest(new { Message = resultMessage });
        }
    }
}
