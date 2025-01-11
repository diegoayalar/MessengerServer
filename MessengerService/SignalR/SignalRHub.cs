using Firebase.Auth;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerService.DTO;
using MessengerService.IServices;
using MessengerService.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.SignalR
{
    [Authorize]
    public class SignalRHub: Hub
    {
        private readonly IChatService _chatService;
        private readonly IGenericRepository<UserConnection> _ConnectionRepository;
        private readonly ILogger<SignalRHub> _logger;

        public SignalRHub(IChatService chatService, IGenericRepository<UserConnection> ConnectionRepository,
                            ILogger<SignalRHub> logger) { 
            _chatService = chatService;
            _ConnectionRepository = ConnectionRepository;
            _logger = logger;
        }

        public async Task SendMessage(string chatID, NewMessageDTO message) {
            var chat = await _chatService.GetChatById(chatID);

            if (chat.Users.Contains(message.Sender))
            {
                await _chatService.AddMessageToChat(message, chat.Id);
                await Clients.Group(chatID).SendAsync("ReceiveMessage", message, chat.Id);
            }
            else {
                _logger.LogInformation("Usuario no tiene permiso en este chat.");
            }

        }

        public override async Task OnConnectedAsync()
        {
            //Se optiene el id del usuario por parametro
            //Esto se puede reemplazar por el TOKEN
            //var userID = GetUserIdFromToken();
            var userId = Context.User?.FindFirst("user_id")?.Value;

            Console.WriteLine("El dispositivo del usuario tiene esta ID:");
            Console.WriteLine(Context.ConnectionId);
            
            var connection = new UserConnection
            {
                id = Guid.NewGuid().ToString(),
                UserId = userId,
                ConnectionId = Context.ConnectionId
            };

            await _ConnectionRepository.InsertAsync(connection);

            var chats = await _chatService.GetAllChatsAsync();

            //Se filtran todos los chats en los que pertenece el usuario.
            var userChats = chats
                .Where(chat => chat.Users != null && chat.Users.Contains(userId))
                .Select(chat => chat)
                .ToList();


            //Se añade su conexión a cada chat.
            foreach (var chat in userChats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id);

            }

            await base.OnConnectedAsync();
            Console.WriteLine($"New connection: {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = await _ConnectionRepository.GetByFieldAsync("ConnectionId",Context.ConnectionId);
            if (connection != null)
            {
                // Eliminar la conexión de la base de datos
                await _ConnectionRepository.DeleteByFieldAsync("ConnectionId", connection.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"Connection lost: {Context.ConnectionId}");
        }
    }

}
