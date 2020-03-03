using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using MessageBusFun.Core.Messages;

namespace MessageBusFun.Core.Tcp
{
    public sealed class MesageBusTcpServer
    {
        X509Certificate _serverCertificate = null;
        private List<MessageBusTcpClient> _connectedClients = new List<MessageBusTcpClient>();
        TcpListener _listener;
        public int ConnectedClientCount => _connectedClients.Count;
        public event EventHandler<TcpClientData> DataReceived;

        public void Start(string ipAddress, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _listener.Start();
            BeginAcceptClient();
            Console.WriteLine("Server started");
        }

        public void Start(string ipAddress, int port, string certificate)
        {
            _serverCertificate = X509Certificate2.CreateFromCertFile(certificate);
            Start(ipAddress, port);
        }

        private void BeginAcceptClient()
        {
            _listener.BeginAcceptTcpClient(new AsyncCallback(AcceptClientCallback), null);
        }

        private void AcceptClientCallback(IAsyncResult ar)
        {
            var client = _listener.EndAcceptTcpClient(ar);
            client.ReceiveTimeout = 5000;
            client.SendTimeout = 5000;
            var mbClient = new MessageBusTcpClient(client);
            _connectedClients.Add(mbClient);

            var stream = client.GetStream();
            var state = new ConnectionState() { Client = mbClient, Stream = stream };
            try
            {
                stream.BeginRead(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(ReceiveCallback), state);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //TODO: debug sslStream AuthenticateAsServer exception
            //var sslStream = new SslStream(client.GetStream(), false);
            //try
            //{
            //    sslStream.AuthenticateAsServer(_serverCertificate, false, SslProtocols.Tls, true);

            //    var state = new ConnectionState() { Client = client, Stream = sslStream };

            //    sslStream.BeginRead(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(ReceiveCallback), state);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}

            BeginAcceptClient();
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
                    state.SB.Clear();
                    DataReceived?.Invoke(this, new TcpClientData(state.Buffer.Take(byteCount).ToArray(), state.Client.Id));
                    state.Stream.BeginRead(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(ReceiveCallback), state);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void SendMessage(byte[] messageBytes, int clientId)
        {
            var client = _connectedClients.FirstOrDefault(p => p.Id == clientId);
            if (client == null) return;

            var state = new ConnectionState();
            state.Buffer = messageBytes;
            state.Stream = client.GetStream();

            state.Stream.BeginWrite(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(WriteCallback), state);
        }

        public void SendMessage(Message message, int clientId)
        {
            var client = _connectedClients.FirstOrDefault(p => p.Id == clientId);
            if (client == null) return;

            var state = new ConnectionState();
            state.Message = message;
            state.Buffer = message.ToByteArray();
            state.Stream = client.GetStream();

            state.Stream.BeginWrite(state.Buffer, 0, state.Buffer.Length, new AsyncCallback(WriteCallback), state);
        }

        private void WriteCallback(IAsyncResult ar)
        {
            var state = (ConnectionState)ar.AsyncState;

            state.Stream.EndWrite(ar);
        }

        public void CloseTcpClients()
        {
            foreach (var client in _connectedClients)
            {
                client.Close();
            }
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}
