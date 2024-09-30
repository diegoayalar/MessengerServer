using MessengerDomain.Entities;
using MessengerService.Service;
using Microsoft.AspNetCore.Mvc;

namespace MessengerServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatControllerr : Controller
    {
        private readonly ChatService _chatService;

        public ChatControllerr(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] Chat chat)
        {
            await _chatService.InsertUserAsync(chat);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> user()
        {
            return Ok("Hola puta");
        }
    }
}
