using Firebase.Auth;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerService.DTO;
using MessengerService.Service;
using MessengerService.Util;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.SignalR
{
    public class SignalRHub: Hub
    {

        private readonly ChatService _chatService;
        private readonly IGenericRepository<UserConnection> _ConnectionRepository;

        public SignalRHub(ChatService chatService, IGenericRepository<UserConnection> ConnectionRepository) { 
            _chatService = chatService;
            _ConnectionRepository = ConnectionRepository;
        }

        public async Task SendMessage(string chatID, NewMessageDTO message) {
            var chat = await _chatService.GetChatById(chatID);
            Console.WriteLine(chat.Id);
            await _chatService.AddMessageToChat(message, chat.Id);
            await Clients.Group(chatID).SendAsync("ReceiveMessage", message, chat.Id);
        }

        public override async Task OnConnectedAsync()
        {
            //Se optiene el id del usuario por parametro
            //Esto se puede reemplazar por el TOKEN
            var userID = Context.GetHttpContext()?.Request.Query["userId"];

            var connection = new UserConnection
            {
                id = Guid.NewGuid().ToString(),
                UserId = userID,
                ConnectionId = Context.ConnectionId
            };

            await _ConnectionRepository.InsertAsync(connection);

            var allChats = await _chatService.GetAllChatsAsync();

            //Se filtran todos los chats en los que pertenece el usuario.
            var userChats = allChats
                .Where(chat => chat.Users != null && chat.Users.Contains(userID))
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
