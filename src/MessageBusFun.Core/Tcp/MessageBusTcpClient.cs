using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading.Tasks;
using MessageBusFun.Core.Messages;

namespace MessageBusFun.Core.Tcp
{
    public sealed class MessageBusTcpClient
    {
        private TcpClient _client;
        private static int _idCount = 0;
        public int Id { get; private set; }
        public event EventHandler<byte[]> DataReceived;
        public Message Message { get; set; }
        public MessageBusTcpClient()
        {

        }

        public MessageBusTcpClient(TcpClient client)
        {
            _client = client;
            Id = _idCount++;
        }

        public void Start(string ipAddress, int port)
        {
            _client = new TcpClient();
            _client.BeginConnect(ipAddress, port, new AsyncCallback(ConnectCallback), _client);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            if (!_client.Connected)
            {
                Console.WriteLine("Couldn't connect to server");
                return;
            }
            

            var state = new ConnectionState() { Stream = GetStream() };
            try
            {
                state.Stream.BeginRead(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Connected to server");
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = (ConnectionState)ar.AsyncState;
            int byteCount = -1;

            byteCount = state.Stream.EndRead(ar);

            Decoder decoder = Encoding.UTF8.GetDecoder();
            char[] chars = new char[decoder.GetCharCount(state.Buffer, 0, byteCount)];
            decoder.GetChars(state.Buffer, 0, byteCount, chars, 0);
            state.SB.Append(chars);

            try
            {
                if (state.SB.ToString().IndexOf(Message.EOF) == -1 && byteCount != 0)
                {
                    state.Stream.BeginRead(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(ReceiveCallback), state);
                }
                else if (byteCount == 0)
                {
                    state.Stream.Close();
                    Console.WriteLine("Client disconnected!");
                }
                else
                {
                    Message = MessageFactory.Create(state.Buffer.Take(byteCount).ToArray());
                    DataReceived?.Invoke(this, state.Buffer.Take(byteCount).ToArray());
                    state.Stream.BeginRead(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Close()
        {
            _client.Close();
        }

        public void SendMessage(byte[] messageBytes)
        {
            if (!_client.Connected) return;

            var state = new ConnectionState();
            state.Buffer = messageBytes;
            state.Stream = _client.GetStream();

            state.Stream.BeginWrite(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(WriteCallback), state);
        }

        public void SendMessage(Message message)
        {
            if (!_client.Connected) return;

            var state = new ConnectionState();
            state.Message = message;
            state.Buffer = message.ToByteArray();
            state.Stream = _client.GetStream();

            state.Stream.BeginWrite(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(WriteCallback), state);
        }

        private void WriteCallback(IAsyncResult ar)
        {
            var state = (ConnectionState)ar.AsyncState;
            Console.WriteLine("Sent message: " + state.Message);

            state.Stream.EndWrite(ar);
        }

        public Stream GetStream()
        {
            return _client.GetStream();
        }
    }
}
