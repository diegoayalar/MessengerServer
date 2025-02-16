using MessengerDomain.Entities;
using MessengerService.DTO;

namespace MessengerService.IServices
{
    public interface IChatService
    {
        Task InsertNewChat(NewChatRequestDTO newChat, Stream? profilePictureStream);
        Task AddMessageToChat(NewMessageDTO message, string chatId);
        Task<IEnumerable<Message>> GetFilteredMessages(string parentID, int size);
        Task EditChat(UpdateChatRequest newChat, string chatId, Stream? profilePictureStream);
        Task EditMessageFromChat(UpdateMessageDTO newMessage, string chatId, string messageId);
        Task RemoveUserFromChat(List<string>? usersIDs, string chatId);
        Task AddUserToChat(List<string> usersIDs, string chatId);
        Task AddAdminUserToChat(string userSender, string userAdminID, string chatId);
        Task RemoveAdminUserToChat(string userSender, string userAdminID, string chatId);
        Task<IEnumerable<Chat>> GetAllChatsAsync();
        Task<IEnumerable<Chat>> GetChatsByUserIdAsync(string userId);
        Task<Chat> GetChatById(string id);
        Task UpdateUserAsync(Chat chat);
        Task DeleteUserAsync(string id);
    }
}