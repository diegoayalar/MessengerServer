
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerEntity.Entity
{
    public class Profile
    {
        public required string Name {  get; set; }

        public string? Description { get; set; }

        public string? ProfilePic {  get; set; }

        public int Status {  get; set; }    

    }
}
