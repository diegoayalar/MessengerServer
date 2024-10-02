using MessengerDomain.Entities;
using MessengerService.DTO;
using System;
using System.Collections.Generic;
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
                IsGroup = newChat.IsGroup
            };
        }
    }
}
