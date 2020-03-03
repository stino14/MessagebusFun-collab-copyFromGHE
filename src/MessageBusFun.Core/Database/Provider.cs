using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBusFun.Core.Database
{
    public class Provider
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public int ClientId { get; set; }
        public int ChannelId { get; set; }
    }
}
