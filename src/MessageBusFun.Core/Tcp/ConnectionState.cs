using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;
using MessageBusFun.Core.Messages;
using MessageBusFun.Core.Tcp;

namespace MessageBusFun.Core.Tcp
{
    public class ConnectionState
    {
        public MessageBusTcpClient Client { get; set; }
        public Stream Stream { get; set; }
        public byte[] Buffer { get; set; }
        public StringBuilder SB { get; set; }
        public Message Message { get; set; }

        public ConnectionState()
        {
            Buffer = new byte[2048];
            SB = new StringBuilder(Buffer.Length);
        }
    }
}
