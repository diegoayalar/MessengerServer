using MessengerDomain.Entities;
using MessengerService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.Util.Mapper
{
    public static class MessageMapper
    {
        public static Message NewMessageDTOToMessage(NewMessageDTO messageDTO)
        {

            return new Message
            {
                MessageText = messageDTO.MessageText,
                File = messageDTO.File,
                Sender = messageDTO.Sender
            };
        }

        public static Message UpdateMessage(Message oldMessage, UpdateMessageDTO newMessageDTO)
        {
            return new Message
            {
                _Id = oldMessage._Id,
                MessageText = newMessageDTO.MessageText,
                Sender = oldMessage.Sender, 
                File = oldMessage.File,
                ReadState = oldMessage.ReadState, 
                RecivedUsers = oldMessage.RecivedUsers,
                UnrecivedUsers = oldMessage.UnrecivedUsers,
                ReadUsers = oldMessage.ReadUsers,
                UneadUsers = oldMessage.UneadUsers,
                DateSent = oldMessage.DateSent 
            };
        }
    }
}
