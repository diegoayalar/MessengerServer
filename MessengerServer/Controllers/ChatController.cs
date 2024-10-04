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

        [HttpPatch("/chat/{chatId}/addMessage")]
        public async Task<IActionResult> AddMessageToChat(string chatId, [FromBody] NewMessageDTO message)
        {
            _logger.LogInformation("ChatController: AddMessageToChat.");
            await _chatService.AddMessageToChat(message, chatId );
            return Ok();
        }

        [HttpPatch("/editChat/{id}")]
        public async Task<IActionResult> EditChat(string id, [FromBody] NewChatRequestDTO newInfoChat) {
            _logger.LogInformation("ChatController: EditChat.");
            await _chatService.EditChat(newInfoChat, id);
            return Ok();
        }

        [HttpPatch("/editMessage/chat/{chatId}/messages/{messageID}")]
        public async Task<IActionResult> EditMessageFromChat(string chatId, string messageID ,[FromBody] UpdateMessageDTO newInfoChat)
        {
            _logger.LogInformation("ChatController: EditChat.");
            await _chatService.EditMessageFromChat(newInfoChat, chatId, messageID);
            return Ok();
        }

        [HttpPatch("/removeUser/chat/{id}")]
        public async Task<IActionResult> RemoveUserFromChat(string id, [FromBody] List<string> usersID)
        {
            _logger.LogInformation("ChatController: RemoveUserFromChat.");
            await _chatService.RemoveUserFromChat(usersID, id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> get() { 
            return Ok(await _chatService.GetAllChatsAsync());
        }

        [HttpGet("/getFilteredMessage/chat/{chatID}")]
        public async Task<IActionResult> GetFilteredMessages(string chatID, [FromQuery] int size)
        {
            _logger.LogInformation("ChatController: GetFilteredMessages.");
            return Ok(await _chatService.getFilteredMessage(chatID,size));
        }
    }
}
