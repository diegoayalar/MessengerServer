using MessengerDomain.Entities;
using MessengerService.DTO;
using MessengerService.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;
        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost("add-chat")]
        public async Task<IActionResult> AddChat([FromForm] NewChatRequestDTO chat, IFormFile? groupPicFile)
        {
            _logger.LogInformation("ChatController: AddChat.");
            Stream? fileStream = groupPicFile != null ? groupPicFile.OpenReadStream() : null;
            await _chatService.InsertNewChat(chat, fileStream);
            return Ok();
        }

        //[HttpPatch("{chatId}/add-message")]
        //public async Task<IActionResult> AddMessageToChat(string chatId, [FromBody] NewMessageDTO message)
        //{
        //    _logger.LogInformation("ChatController: AddMessageToChat.");
        //    await _chatService.AddMessageToChat(message, chatId );
        //    return Ok();
        //}

        [HttpPatch("{chatId}/add-admin/{adminId}")]
        public async Task<IActionResult> AddAdminToChat([FromBody] string sender, string adminId, string chatId)
        {
            _logger.LogInformation("ChatController: AddAdminToChat.");
            await _chatService.AddAdminUserToChat(sender, adminId, chatId);
            return Ok();
        }

        [HttpPatch("{chatId}/remove-admin/{adminId}")]
        public async Task<IActionResult> RemoveAdminToChat([FromBody] string sender, string adminId, string chatId)
        {
            _logger.LogInformation("ChatController: RemoveAdminToChat.");
            await _chatService.RemoveAdminUserToChat(sender, adminId, chatId);
            return Ok();
        }

        [HttpPatch("edit-chat/{id}")]
        public async Task<IActionResult> EditChat(string id, [FromForm] UpdateChatRequest newInfoChat, IFormFile? groupPicFile)
        {
            _logger.LogInformation("ChatController: EditChat.");
            Stream? fileStream = groupPicFile != null ? groupPicFile.OpenReadStream() : null;
            await _chatService.EditChat(newInfoChat, id, fileStream);
            return Ok();
        }

        [HttpPatch("edit-message/chat/{chatId}/messages/{messageID}")]
        public async Task<IActionResult> EditMessageFromChat(string chatId, string messageID, [FromBody] UpdateMessageDTO newInfoChat)
        {
            _logger.LogInformation("ChatController: EditChat.");
            await _chatService.EditMessageFromChat(newInfoChat, chatId, messageID);
            return Ok();
        }

        [HttpPatch("remove-user/chat/{id}")]
        public async Task<IActionResult> RemoveUserFromChat(string id, [FromBody] List<string> usersID)
        {
            _logger.LogInformation("ChatController: RemoveUserFromChat.");
            await _chatService.RemoveUserFromChat(usersID, id);
            return Ok();
        }

        [HttpPatch("add-user/chat/{id}")]
        public async Task<IActionResult> AddUserFromChat(string id, [FromBody] List<string> usersID)
        {
            _logger.LogInformation("ChatController: AddUserFromChat.");
            await _chatService.AddUserToChat(usersID, id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> get()
        {
            return Ok(await _chatService.GetAllChatsAsync());
        }

        //[HttpGet("by-user")]
        //public async Task<IActionResult> GetChatsByUserId()
        //{
        //    try
        //    {
        //        if (!Request.Cookies.TryGetValue("AuthToken", out var token))
        //            return BadRequest(new { Error = "No authentication token found." });

        //        var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        //        var userId = decodedToken.Uid;

        //        var chats = await _chatService.GetChatsByUserIdAsync(userId);

        //        return Ok(chats);
        //    }
        //    catch (FirebaseAdmin.Auth.FirebaseAuthException ex)
        //    {
        //        return Unauthorized(new { Error = "Invalid or expired token.", Details = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Error = "An internal error occurred.", Details = ex.Message });
        //    }
        //}

        [HttpGet("{chatId}")]
        public async Task<IActionResult> getChatByID(string chatId)
        {
            _logger.LogInformation("ChatController: getChatByID.");
            Chat chat = await _chatService.GetChatById(chatId);
            if (chat == null)
            {
                return NotFound("Chat no encontrado.");
            }
            return Ok(chat);
        }

        [HttpGet("get-specific-message/chat/{chatID}")]
        public async Task<IActionResult> GetFilteredMessages(string chatID, [FromQuery] int size)
        {
            _logger.LogInformation("ChatController: GetFilteredMessages.");
            return Ok(await _chatService.GetFilteredMessages(chatID, size));
        }
    }
}
