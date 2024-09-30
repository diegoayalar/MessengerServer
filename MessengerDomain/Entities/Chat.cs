
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerEntity.Entity
{
    public class Chat
    {
        public ICollection<Message>? Messages { get; set; }

        public required ICollection<string> Users { get; set; }

        public string ChatName { get; set; } = "New chat";

        public string? Description { get; set; }

        public string? GroupPic { get; set; }

        public bool IsGroup { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}
