using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.DTO
{
    public class NewChatRequestDTO
    {
        public string GroupPic { get; }

        public ICollection<string> UsersIDs { get; }

        public string Description { get; }

        public bool IsGroup { get; }
    }
}
