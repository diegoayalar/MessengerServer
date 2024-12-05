using LiteDB;
using MessengerDomain.Entities;
using MessengerPersistency.IRepository;
using MessengerPersistency.Repository;
using MessengerService.DTO;
using MessengerService.SignalR;
using MessengerService.Util;
using MessengerService.Util.Mapper;
using Microsoft.AspNetCore.SignalR;
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
        private readonly FirebaseStorageService _firebaseStorageService;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly IGenericRepository<UserConnection> _userConnectionRepository;

        public ChatService(IGenericRepository<Chat> Repository, ILogger<ChatService> logger, 
                FirebaseStorageService firebaseStorageService, IHubContext<SignalRHub> hubContext,
                IGenericRepository<UserConnection> genericRepository
                )
        {
            _chatRepository = Repository;
            _logger = logger;
            _firebaseStorageService = firebaseStorageService;
            _hubContext = hubContext;
            _userConnectionRepository = genericRepository;
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
                var response = await _chatRepository.InsertAsync(chat);
                chat.Id = response.Key;
                await _chatRepository.UpdateAsync(chat);

                foreach (var participant in newChat.UsersIDs)
                {
                    var connections = await _userConnectionRepository.GetAllAsync();
                    var filteredChats = connections
                        .Where(con => con.UserId != null && con.UserId.Contains(participant))
                        .Select(con => con);

                    foreach (var connection in connections)
                    {
                        await _hubContext.Groups.AddToGroupAsync(connection.ConnectionId, chat.Id);
                    }
                }

                //_signalRHub.CreateGroup(chat.Id, chat.Users.ToList());
                _logger.LogInformation("Se ha creado el nuevo chat correctamente.");
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar un nuevo chat..");
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
                //await _signalRHub.SendMessage(chatId, message);
                _logger.LogInformation(" Se ha agregado correctamente el mensaje.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar un mensaje a el chat.");
            }
        }


        public async Task AddAdminUserToChat(string userSender, string userAdminID, string chatId)
        {
            _logger.LogInformation("Iniciando la incerción de un nuevo Admin a el chat.");
            try
            {
                var chat = await _chatRepository.GetByIdAsync(chatId);

                if (!chat.AdminUsers.Contains(userSender))
                    throw new UnauthorizedAccessException("El usuario no tiene permisos de administrador en el chat para realizar esta acción");
                
                chat.AdminUsers.Add(userAdminID);
                await _chatRepository.UpdateAsync(chat);
                _logger.LogInformation(" Se ha agregado correctamente el Admin.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar un Admin a el chat.");
            }
        }

        public async Task RemoveAdminUserToChat(string userSender, string userAdminID, string chatId)
        {
            _logger.LogInformation("Iniciando la revocación de un Admin a el chat.");
            try
            {
                var chat = await _chatRepository.GetByIdAsync(chatId);

                if (!chat.AdminUsers.Contains(userSender))
                    throw new UnauthorizedAccessException("El usuario no tiene permisos de administrador en el chat para realizar esta acción");

                chat.AdminUsers.Remove(userAdminID);
                await _chatRepository.UpdateAsync(chat);
                _logger.LogInformation(" Se ha revocado correctamente el mensaje.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al revocar un admin del chat.");
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

        public async Task RemoveUserFromChat(List<string> usersIDs, string chatId)
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

                    foreach (var user in usersIDs)
                    {
                        var connectionUser = await _userConnectionRepository.GetByFieldAsync("UserId", user);
                        if (connectionUser != null) _hubContext.Groups.RemoveFromGroupAsync(connectionUser.ConnectionId, chatId);
                    }
                }

                await _chatRepository.UpdateAsync(chat);

                _logger.LogInformation("Se han removido correctamente los usuarios seleccionados del chat.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover un usuario del chat.");
            }
        }

        public async Task AddUserToChat(List<string> usersIDs, string chatId)
        {
            _logger.LogInformation("Iniciando la agregación de usuarios a un chat.");
            try
            {
                Chat chat = await GetChatById(chatId);
                chat.Id = chatId;
                if (chat == null)
                {
                    throw new ArgumentException("No se ha encontrado ningún chat, verifique la información.");
                }

                //Agrega los nuevos usuarios a el chat.
                foreach (var newMember in usersIDs) 
                {
                    chat.Users.Add(newMember);

                    var connectionUser = await _userConnectionRepository.GetByFieldAsync("UserId", newMember);
                    if (connectionUser != null) await _hubContext.Groups.AddToGroupAsync(connectionUser.ConnectionId, chatId);
                }

                await _chatRepository.UpdateAsync(chat);

                _logger.LogInformation(" Se han agregado correctamente los usuarios seleccionados del chat.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar los usuarios al chat.");
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
