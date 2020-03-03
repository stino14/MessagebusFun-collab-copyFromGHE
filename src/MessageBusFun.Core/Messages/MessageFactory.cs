using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBusFun.Core.Messages
{
    public static class MessageFactory
    {
        public static Message Create(byte[] message)
        {
            var messageString = Encoding.ASCII.GetString(message);
            var tag = messageString.Split(Message.Delimiter).FirstOrDefault();
            Message msg = null;
            if (tag == MessageType.Channel.ToString())
            {
                msg = new ChannelMessage();
            }
            else if (tag == MessageType.Registration.ToString())
            {
                msg = new RegistrationMessage();
            }
            else if (tag == MessageType.GetChannels.ToString())
            {
                msg = new RequestChannelsMessage();
            }
            else if (tag == MessageType.ChannelUnavailable.ToString())
            {
                msg = new ChannelUnavailableMessage();
            }

            msg?.ParseMessage(message);
            return msg;
        }
    }
}
