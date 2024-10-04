using LiteDB;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerService.DTO;
using MessengerService.Util.Mapper;
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
            try {
                _logger.LogInformation("Iniciando la incerción de un nuevo chat.");
                var chat = ChatMapper.NewChatRequestToChat(newChat);
                await _chatRepository.InsertAsync(chat);
                _logger.LogInformation("Se ha creado el nuevo chat correctamente.");
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar un nuevo chat.");
            }
        }

        public async Task InsertNewChat2(Chat newChat)
        {
            try
            {
                _logger.LogInformation("Iniciando la incerción de un nuevo chat.");
                //var chat = ChatMapper.NewChatRequestToChat(newChat);
                await _chatRepository.InsertAsync(newChat);
                _logger.LogInformation("Se ha creado el nuevo chat correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar un nuevo chat.");
            }
        }

        public async Task AddMessageToChat(NewMessageDTO message, string chatId)
        {
            _logger.LogInformation("Iniciando la incerción de un nuevo mensaje en el chat.");
            try
            {
                var newMessageId = Guid.NewGuid().ToString();
                var newMessage = MessageMapper.NewMessageDTOToMessage(message);
                newMessage._Id = newMessageId;

                newMessage.UnrecivedUsers = new List<string>(); 
                newMessage.UneadUsers = new List<string>();
                newMessage.ReadUsers = new List<string>();
                newMessage.RecivedUsers = new List<string>();

                newMessage.ReadUsers.Add(newMessage.Sender);
                newMessage.RecivedUsers.Add(newMessage.Sender);

                newMessage.UneadUsers.Add(newMessage.Sender);
                newMessage.UnrecivedUsers.Add(newMessage.Sender);
                await _chatRepository.UpdateOrAddChildItem(chatId,"Messages", newMessageId, newMessage);
                _logger.LogInformation(" Se ha agregado correctamente el mensaje.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar un mensaje a el chat.");
            }
        }

        public async Task<IEnumerable<Message>> getFilteredMessage(string parentID, int size) 
        {
            _logger.LogInformation("Iniciando la busqueda y retorno de los mensajes en el chat.");

            try 
            {
                return await _chatRepository.getFiltredItems<Message>(parentID, "Messages", size);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al retornar los mensajes del chat.");
                return null;
            }
        }

        public async Task EditChat(NewChatRequestDTO newChat, string chatId)
        {
            _logger.LogInformation("Iniciando la edición de un chat.");
            try
            {
                Chat chat = await GetChatById(chatId);
                chat.Id = chatId;
                if (chat == null)
                {
                    throw new ArgumentException("No se ha encontrado ningún chat, verifique la información.");
                }

                var UpdatedChat = ChatMapper.UpdateChat(chat, newChat);
                await _chatRepository.UpdateAsync(chat);
                _logger.LogInformation(" Se ha actualizado correctamente el chat.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar un chat.");
            }
        }

        public async Task EditMessageFromChat(UpdateMessageDTO newChat, string chatId,string messageId)
        {
            _logger.LogInformation("Iniciando la edición de un message.");
            try
            {
                var oldMessage = await _chatRepository.GetChildItem<Message>(chatId, "Messages", messageId);

                if (oldMessage == null)
                {
                    throw new ArgumentException("No se ha encontrado el Message que desea actualizar.");
                }

                var updatedMessage = MessageMapper.UpdateMessage(oldMessage, newChat);

                await _chatRepository.UpdateOrAddChildItem(chatId, "Messages", messageId, updatedMessage);
                _logger.LogInformation("Se ha actualizado correctamente el Message.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar un message del chat.");
            }
        }

        public async Task RemoveUserFromChat(List<string>? usersIDs, string chatId)
        {
            _logger.LogInformation("Iniciando la eliminación de usuarios de un chat.");
            try
            {
                Chat chat = await GetChatById(chatId);
                chat.Id = chatId;
                if (chat == null)
                {
                    throw new ArgumentException("No se ha encontrado ningún chat, verifique la información.");
                }

                // Elimina todos los usuarios seleccionados.
                if (usersIDs != null && usersIDs.Any() && chat.Users != null) {
                    var usersSet = new HashSet<string>(chat.Users);
                    usersSet.ExceptWith(usersIDs);
                    chat.Users = usersSet.ToList();
                }

                await _chatRepository.UpdateAsync(chat);
                _logger.LogInformation(" Se han removido correctamente los usuarios seleccionados del chat.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover un usuario del chat.");
            }
        }
        public async Task<IEnumerable<Chat>> GetAllUsersAsync() => await _chatRepository.GetAllAsync();
        public async Task<Chat> GetChatById(string id) {
            try {
                _logger.LogInformation("Iniciando la busqueda de un chat.");
                var chat = await _chatRepository.GetByIdAsync(id);

                if (chat == null) {
                    throw new ArgumentException("No se ha encontrado ningún chat, verifique la información.");
                }
                return chat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar un chat.");
                return null;
            }

        }
        public async Task UpdateUserAsync(Chat chat) => await _chatRepository.UpdateAsync(chat);
        public async Task DeleteUserAsync(string id) => await _chatRepository.DeleteAsync(id);

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _chatRepository.GetAllAsync();
        }
    }
}
