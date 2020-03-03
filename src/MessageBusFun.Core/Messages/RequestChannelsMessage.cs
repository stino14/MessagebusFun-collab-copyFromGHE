using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MessageBusFun.Core.Messages
{
    public class RequestChannelsMessage : Message
    {
        private char _delimiter = ';';
        public List<string> Channels { get; set; } = new List<string>();

        public RequestChannelsMessage()
        {
            MessageType = MessageType.GetChannels;
        }

        public override void ParseMessage(byte[] message)
        {
            var messageStringArray = GetMessageStringArray(message);
            Channels = messageStringArray[3].Split(_delimiter).ToList();
            base.ParseMessage(message);
        }

        public override byte[] ToByteArray()
        {
            return ToByteArray(new List<string>() { string.Join(_delimiter, Channels.ToArray()) }.ToArray());
        }
    }
}
