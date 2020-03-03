using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBusFun.Core.Database
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public int ClientId { get; set; }
        public int ChannelId { get; set; }
        public string Channel { get; set; }
    }
}
