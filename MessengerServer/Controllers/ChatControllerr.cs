using MessengerDomain.Entities;
using MessengerService.DTO;
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
        public async Task<IActionResult> RegisterUser([FromBody] NewChatRequestDTO chat)
        {
            await _chatService.InsertNewChat(chat);
            return Ok();
        }

        [HttpPatch("/addMessage/{id}")]
        public async Task<IActionResult> addMessageToChat(string id,[FromBody] NewMessageDTO message)
        {
            await _chatService.AddMessageToChat(message, id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> user()
        {
            return Ok("Hola puta");
        }
    }
}
