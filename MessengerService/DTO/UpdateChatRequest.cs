using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.DTO
{
    public class UpdateChatRequest 
    {
        public string? ChatName { get; set; }

        public string? Description { get; set; }

    }
}
