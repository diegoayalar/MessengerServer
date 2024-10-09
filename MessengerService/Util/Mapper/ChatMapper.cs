using MessengerDomain.Entities;
using MessengerService.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.Util.Mapper
{
    public static class ChatMapper
    {
        public static Chat NewChatRequestToChat(NewChatRequestDTO newChat, string nameFile)
        {
            return new Chat
            {
                GroupPic = nameFile,
                Users = newChat.UsersIDs,
                Description = newChat.Description,
                IsGroup = newChat.IsGroup,
                ChatName = newChat.ChatName
            };
        }

        public static Chat UpdateChat(Chat oldChat, UpdateChatRequest newChat, string ImageName)
        {
            oldChat.GroupPic = ImageName;
            oldChat.ChatName = newChat.ChatName;
            oldChat.Description = newChat.Description;

            return oldChat;
        }
    }
}
