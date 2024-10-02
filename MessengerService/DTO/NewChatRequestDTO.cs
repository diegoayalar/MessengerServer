using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.DTO
{
    public class NewChatRequestDTO
    {
        public string? GroupPic { get; set; }

        public ICollection<string>? UsersIDs { get; set; }

        public string? ChatName { get; set; }

        public string? Description { get; set; }

        public bool IsGroup { get; set; }
    }
}
