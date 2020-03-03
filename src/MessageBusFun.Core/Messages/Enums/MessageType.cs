using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBusFun.Core.Messages
{
    public enum MessageType
    {
        Channel,
        Auth,
        Registration,
        RegisterProvider,
        UnregisterProvider,
        RegisterSubscriber,
        UnregisterSubscriber,
        GetChannels,
        ChannelUnavailable
    }
}
