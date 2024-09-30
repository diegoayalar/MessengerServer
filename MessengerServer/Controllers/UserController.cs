using MessengerDomain.Entities;
using MessengerService.Service;
using Microsoft.AspNetCore.Mvc;

namespace MessengerServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            await _userService.InsertUserAsync(user);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> user()
        {
            return Ok("Hola puta");
        }
    }
}
