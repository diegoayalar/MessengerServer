using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerService.DTO
{
    public class NewMessageDTO
    {
        public string? MessageText { get; set; }
        public string? File { get; set; }
        public string? Sender { get; set; }
        public DateTime SentAt {  get; set; }   
    }
}
