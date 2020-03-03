using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBusFun.Core.Database
{
    public class Message
    {
        public int Id { get; set; } //Used for DB only
        public int ChannelId { get; set; }  //Used for DB only
        public string Channel { get; set; }
        public int SubscriberId { get; set; }  //Not used until DB support is provided
        public string ByteMessageString { get; set; } //Used for DB storage instead of Byte array
        public int ClientId { get; set; } //Temporary until DB support is provided using SubscriberID allowing clients to connect/disconnect without missing messages
    }
}
