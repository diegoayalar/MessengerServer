using MessengerDomain.Entities;
using MessengerService.DTO;
using MessengerService.Service;
using Microsoft.AspNetCore.Mvc;

namespace MessengerServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;
        private readonly ILogger<ChatController> _logger;
        public ChatController(ChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost("/addNewChat")]
        public async Task<IActionResult> AddChat([FromBody] NewChatRequestDTO chat)
        {
            await _chatService.InsertNewChat(chat);
            return Ok();
        }

        [HttpPatch("/addMessage/{id}")]
        public async Task<IActionResult> AddMessageToChat(string id,[FromBody] NewMessageDTO message)
        {
            _logger.LogInformation("ChatController: AddMessageToChat.");
            await _chatService.AddMessageToChat(message, id);
            return Ok();
        }

        [HttpPatch("/editChat/{id}")]
        public async Task<IActionResult> EditChat(string id, [FromBody] NewChatRequestDTO newInfoChat) {
            _logger.LogInformation("ChatController: EditChat.");
            await _chatService.EditChat(newInfoChat, id);
            return Ok();
        }

        [HttpPatch("/editMessage/chat/{id}")]
        public async Task<IActionResult> EditMessageFromChat(string id, [FromBody] UpdateMessageDTO newInfoChat)
        {
            _logger.LogInformation("ChatController: EditChat.");
            await _chatService.EditMessageFromChat(newInfoChat, id);
            return Ok();
        }

        [HttpPatch("/removeUser/chat/{id}")]
        public async Task<IActionResult> RemoveUserFromChat(string id, [FromBody] List<string> usersID)
        {
            _logger.LogInformation("ChatController: RemoveUserFromChat.");
            await _chatService.RemoveUserFromChat(usersID, id);
            return Ok();
        }
    }
}
