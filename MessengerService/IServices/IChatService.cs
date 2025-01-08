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
        Task<IEnumerable<Chat>> GetAllChatsAsync();
        Task<Chat> GetChatById(string id);
        Task UpdateUserAsync(Chat chat);
        Task DeleteUserAsync(string id);
    }
}