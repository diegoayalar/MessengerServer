using MessengerDomain.Entities;
using MessengerService.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.Mapper
{
    public static class ChatMapper
    {
        public static Chat NewChatRequestToChat(NewChatRequestDTO newChat) {
            return new Chat
            {
                GroupPic = newChat.GroupPic,
                Users = newChat.UsersIDs,
                Description = newChat.Description,
                IsGroup = newChat.IsGroup,
                ChatName = newChat.ChatName
            };
        }

        public static Chat UpdateChat(Chat oldChat, NewChatRequestDTO newChat)
        {
            oldChat.GroupPic = newChat.GroupPic;
            oldChat.ChatName = newChat.ChatName;
            oldChat.Description = newChat.Description;

            return oldChat;
        }
    }
}
