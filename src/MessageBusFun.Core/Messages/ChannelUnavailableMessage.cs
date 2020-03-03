using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBusFun.Core.Messages
{
    public class ChannelUnavailableMessage : ChannelMessage
    {
        public ChannelUnavailableMessage()
        {
            MessageType = MessageType.ChannelUnavailable;
            MessageString = "Channel Unavailable";
        }
    }
}
