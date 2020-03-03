using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBusFun.Core.Tcp
{
    public class TcpClientData
    {
        public byte[] MessageBytes { get; set; }
        public int ClientID { get; set; }

        public TcpClientData(byte[] messageBytes, int clientID)
        {
            MessageBytes = messageBytes;
            ClientID = clientID;
        }
    }
}
