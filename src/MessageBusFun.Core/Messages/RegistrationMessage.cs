using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBusFun.Core.Messages
{
    public class RegistrationMessage : Message
    {
        public ClientType ClientType { get; set; }
        public RegistrationType RegistrationType { get; set; }
        public string Channel { get; set; }

        public RegistrationMessage()
        {
            MessageType = MessageType.Registration;
        }

        public override void ParseMessage(byte[] message)
        {
            var messageStringArray = GetMessageStringArray(message);
            ClientType = Enum.Parse<ClientType>(messageStringArray[3]);
            RegistrationType = Enum.Parse<RegistrationType>(messageStringArray[4]);
            Channel = messageStringArray[5];
            base.ParseMessage(message);
        }

        public override byte[] ToByteArray()
        {
            return ToByteArray(new List<string>() { ClientType.ToString(), RegistrationType.ToString(), Channel }.ToArray());
        }
    }
}
