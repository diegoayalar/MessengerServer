using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerPersistency.Repository;
using MessengerService.DTO;
using MessengerService.IServices;
using MessengerService.Util.Mapper;
using Microsoft.Extensions.Logging;

namespace MessengerService.Services
{
    public class ChatService : IChatService
    {
        private readonly IGenericRepository<Chat> _chatRepository;
        private readonly ILogger<ChatService> _logger;
        private readonly FirebaseStorageService _firebaseStorageService;
        public ChatService(IGenericRepository<Chat> Repository, ILogger<ChatService> logger, FirebaseStorageService firebaseStorageService)
        {
            _chatRepository = Repository;
            _logger = logger;
            _firebaseStorageService = firebaseStorageService;
        }

        public async Task InsertNewChat(NewChatRequestDTO newChat, Stream? profilePictureStream) {
            try {
                _logger.LogInformation("Iniciando la inserción de un nuevo chat.");

                var nameFile = "";

                if (profilePictureStream != null)
                {
                    _logger.LogInformation("Subiendo nueva imagen de perfil al almacenamiento Firebase.");

                    // Generar un nombre único para la imagen (Se puede cambiar la extensión ej: .png)
                    string newFileName = $"{Guid.NewGuid()}.jpg";
                    nameFile = newFileName;

                    // Subir la nueva imagen al almacenamiento de Firebase
                    await _firebaseStorageService.UploadFileAsync(profilePictureStream, newFileName);

                }
                var chat = ChatMapper.NewChatRequestToChat(newChat, nameFile);
                await _chatRepository.InsertAsync(chat);
                _logger.LogInformation("Se ha creado el nuevo chat correctamente.");
            } catch (Exception ex)
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

        public async Task<IEnumerable<Message>> GetFilteredMessages(string parentID, int size) 
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

        public async Task EditChat(UpdateChatRequest newChat, string chatId, Stream? profilePictureStream)
        {
            _logger.LogInformation("Iniciando la edición de un chat.");
            try
            {
                Chat chat = await _chatRepository.GetByIdAsync(chatId);
                chat.Id = chatId;

                if (chat == null)
                {
                    throw new ArgumentException("No se ha encontrado ningún chat, verifique la información.");
                }

                var newGroupPicUrl = chat.GroupPic;
                if (profilePictureStream != null)
                {
                    _logger.LogInformation("Subiendo nueva imagen de perfil al almacenamiento Firebase.");

                    // Generar un nombre único para la imagen (Se puede cambiar la extensión ej: .png)
                    string newFileName = $"{Guid.NewGuid()}.jpg";

                    // Subir la nueva imagen al almacenamiento de Firebase
                    await _firebaseStorageService.UploadFileAsync(profilePictureStream, newFileName);

                    newGroupPicUrl = newFileName;

                    //Si el chat tenía una imagen anterior, eliminarla del almacenamiento
                    if (!string.IsNullOrEmpty(chat.GroupPic))
                    {
                        _logger.LogInformation("Eliminando la imagen de perfil anterior del almacenamiento Firebase.");
                        await _firebaseStorageService.DeleteFileAsync(chat.GroupPic);
                    }

                }

                var UpdatedChat = ChatMapper.UpdateChat(chat, newChat, newGroupPicUrl);
                await _chatRepository.UpdateAsync(chat);
                _logger.LogInformation("Se ha actualizado correctamente el chat.");

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

                if (!string.IsNullOrEmpty(chat.GroupPic))
                {
                    chat.GroupPic = await _firebaseStorageService.GetFileAsync(chat.GroupPic);
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
