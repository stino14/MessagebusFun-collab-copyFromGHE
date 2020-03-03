using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBusFun.Core.Messages
{
    public class ChannelMessage : Message
    {
        public string Channel { get; set; }
        public string MessageString { get; set; }

        public ChannelMessage()
        {
            MessageType = MessageType.Channel;
        }

        public override void ParseMessage(byte[] message)
        {
            var messageStringArray = GetMessageStringArray(message);
            Channel = messageStringArray[3];
            MessageString = messageStringArray[4];
            base.ParseMessage(message);
        }

        public override byte[] ToByteArray()
        {
            return ToByteArray(new List<string>() { Channel, MessageString }.ToArray());
        }
    }
}
