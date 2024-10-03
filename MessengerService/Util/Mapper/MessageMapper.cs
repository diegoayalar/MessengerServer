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
            oldMessage.MessageText = newMessageDTO.MessageText;

            return oldMessage;
        }
    }
}
