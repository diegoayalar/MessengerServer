using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerService.DTO;
using MessengerService.Mapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.Service
{
    public class ChatService
    {
        private readonly IGenericRepository<Chat> _chatRepository;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IGenericRepository<Chat> Repository, ILogger<ChatService> logger)
        {
            _chatRepository = Repository;
            _logger = logger;
        }

        public async Task InsertNewChat(NewChatRequestDTO newChat) {

            var chat = ChatMapper.NewChatRequestToChat(newChat);
            await _chatRepository.InsertAsync(chat);
        }

        public async Task AddMessageToChat(NewMessageDTO message, string Chat)
        {
            _logger.LogInformation("Iniciando la incerción de un nuevo mensaje en el chat.");
            try 
            {
                Chat chat = await GetChatById(Chat);
                if (chat == null)
                {
                    throw new ArgumentException("No se ha encontrado ningún chat, verifique la información.");
                }
                var newMessage = MessageMapper.NewMessageDTOToMessage(message);
                chat.Messages.Add(newMessage);
                await _chatRepository.UpdateAsync(chat);
                _logger.LogInformation(" Se ha agregado correctamente el mensaje.");
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,"Error al agregar un mensaje a el chat.");
            }
        }
        public async Task<IEnumerable<Chat>> GetAllUsersAsync() => await _chatRepository.GetAllAsync();
        public async Task<Chat> GetChatById(string id) => await _chatRepository.GetByIdAsync(id);
        public async Task UpdateUserAsync(Chat chat) => await _chatRepository.UpdateAsync(chat);
        public async Task DeleteUserAsync(string id) => await _chatRepository.DeleteAsync(id);
    }
}
