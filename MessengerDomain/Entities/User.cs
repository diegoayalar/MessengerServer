
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerEntity.Entity
{
    public class User
    {
        public required string Email { get; set; }

        public required string Password { get; set; }

        public ICollection<string>? Chats { get; set; }

        public string? Phone {  get; set; }

        public DateTime? DateCreated { get; set; }

        public bool IsActive { get; set; } = true;

        public Profile? Profile { get; set; }

    }
}
