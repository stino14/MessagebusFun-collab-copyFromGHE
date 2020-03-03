using System;
using System.Collections.Generic;
using System.Text;
using MessageBusFun.Core.Tcp;
using MessageBusFun.Core.Messages;

namespace MessageBusFun.Core
{
    public class MessageBusClient : IMessageBusClient
    {
        private MessageBusTcpClient _tcpClient;
        private Queue<Database.Message> _sendMessageQueue;  //Todo: store to the client DB to ensure messages are sent regardless of client/server outages

        public static int UserCount = 0; //Remove: testing only to create unique user names
        private string _user; //Todo: pull from app.config
        private string _pwd; //Todo: pull from app.config

        public event EventHandler<ChannelMessageClient> ChannelMessageReceived;
        public event EventHandler<List<string>> ChannelListMessageReceived;

        public List<ChannelMessageClient> ChannelMessageClient { get; set; } = new List<ChannelMessageClient>(); //Remove: testing only
        public List<string> ChannelListMessageClient { get; set; } //Remove: testing only

        //Remove: testing only
        public MessageBusClient()
        {
            _user = "user" + UserCount++;
            _pwd = "pwd";
        }

        public MessageBusClient(string user, string pwd)
        {
            _user = user;
            _pwd = pwd;
        }

        public void Start(string ipAddress, int port)
        {
            _sendMessageQueue = LoadMessageQueue();
            _tcpClient = new MessageBusTcpClient();
            _tcpClient.Start(ipAddress, port);
            _tcpClient.DataReceived += DataReceived;
        }

        private Queue<Database.Message> LoadMessageQueue()
        {
            //Todo: pull from DB
            return new Queue<Database.Message>();
        }

        private void CreateAndSendRegistrationMessage(string channel, ClientType clientType, RegistrationType registrationType)
        {
            var registrationMessage = new RegistrationMessage
            {
                User = _user,
                Password = _pwd,
                Channel = channel,
                ClientType = clientType,
                RegistrationType = registrationType
            };
            _sendMessageQueue.Enqueue(new Database.Message() { Channel = registrationMessage.Channel, ByteMessageString = Encoding.ASCII.GetString(registrationMessage.ToByteArray()) });
            SendMessages();
        }

        public void RegisterProvider(string channel)
        {
            //var registrationMessage = new RegistrationMessage
            //{
            //    User = _user,
            //    Password = _pwd,
            //    Channel = channel,
            //    ClientType = ClientType.Provider,
            //    RegistrationType = RegistrationType.Register
            //};
            //_sendMessageQueue.Enqueue(new Database.Message() { Channel = registrationMessage.Channel, ByteMessageString = Encoding.ASCII.GetString(registrationMessage.ToByteArray()) });
            //SendMessages();
            CreateAndSendRegistrationMessage(channel, ClientType.Provider, RegistrationType.Register);
        }

        public void UnregisterProvider(string channel)
        {
            //var registrationMessage = new RegistrationMessage
            //{
            //    User = _user,
            //    Password = _pwd,
            //    Channel = channel,
            //    ClientType = ClientType.Provider,
            //    RegistrationType = RegistrationType.Unregister
            //};
            //_sendMessageQueue.Enqueue(new Database.Message() { Channel = registrationMessage.Channel, ByteMessageString = Encoding.ASCII.GetString(registrationMessage.ToByteArray()) });
            //SendMessages();
            CreateAndSendRegistrationMessage(channel, ClientType.Provider, RegistrationType.Unregister);
        }

        public void RegisterSubscriber(string channel)
        {
            //var registrationMessage = new RegistrationMessage
            //{
            //    User = _user,
            //    Password = _pwd,
            //    Channel = channel,
            //    ClientType = ClientType.Subscriber,
            //    RegistrationType = RegistrationType.Register
            //};
            //_sendMessageQueue.Enqueue(new Database.Message() { Channel = registrationMessage.Channel, ByteMessageString = Encoding.ASCII.GetString(registrationMessage.ToByteArray()) });
            //SendMessages();
            CreateAndSendRegistrationMessage(channel, ClientType.Subscriber, RegistrationType.Register);
        }

        public void UnregisterSubscriber(string channel)
        {
            //var registrationMessage = new RegistrationMessage
            //{
            //    User = _user,
            //    Password = _pwd,
            //    Channel = channel,
            //    ClientType = ClientType.Subscriber,
            //    RegistrationType = RegistrationType.Unregister
            //};
            //_sendMessageQueue.Enqueue(new Database.Message() { Channel = registrationMessage.Channel, ByteMessageString = Encoding.ASCII.GetString(registrationMessage.ToByteArray()) });
            //SendMessages();
            CreateAndSendRegistrationMessage(channel, ClientType.Subscriber, RegistrationType.Unregister);
        }

        public void PublishMessage(string channel, string messageString)
        {
            var channelMessage = new ChannelMessage() { User = _user, Password = _pwd, Channel = channel, MessageString = messageString };
            _sendMessageQueue.Enqueue(new Database.Message() { Channel = channelMessage.Channel, ByteMessageString = Encoding.ASCII.GetString(channelMessage.ToByteArray()) });
            SendMessages();
        }

        public void RequestChannels()
        {
            var requestChannelsMessage = new RequestChannelsMessage() { User = _user, Password = _pwd };
            _sendMessageQueue.Enqueue(new Database.Message() { ByteMessageString = Encoding.ASCII.GetString(requestChannelsMessage.ToByteArray()) });
            SendMessages();
        }

        private void SendMessages()
        {
            //Todo: change to using an async callback to confirm send and remove from queue and DB
            while (_sendMessageQueue.Count > 0)
            {
                var message = _sendMessageQueue.Dequeue();
                _tcpClient.SendMessage(Encoding.ASCII.GetBytes(message.ByteMessageString));
            }
        }

        //Todo: refactor
        private void DataReceived(object sender, byte[] messageBytes)
        {
            var message = MessageFactory.Create(messageBytes);

            if (message.MessageType == MessageType.Channel)
            {
                var channelMessage = (ChannelMessage)message;
                var channelMessageClient = new ChannelMessageClient() { Channel = channelMessage.Channel, MessageString = channelMessage.MessageString };
                ChannelMessageReceived?.Invoke(this, channelMessageClient);
                ChannelMessageClient.Add(channelMessageClient);  //Remove: testing only
            }
            else if(message.MessageType == MessageType.ChannelUnavailable)
            {
                var channelMessage = (ChannelUnavailableMessage)message;
                var channelMessageClient = new ChannelMessageClient() { Channel = channelMessage.Channel, MessageString = channelMessage.MessageString };
                ChannelMessageReceived?.Invoke(this, channelMessageClient);
                ChannelMessageClient.Add(channelMessageClient);  //Remove: testing only
            }
            else if (message.MessageType == MessageType.GetChannels)
            {
                var requestChannelsMessage = (RequestChannelsMessage)message;
                ChannelListMessageReceived?.Invoke(this, requestChannelsMessage.Channels);
                ChannelListMessageClient = requestChannelsMessage.Channels;  //Remove: testing only
            }
        }
    }
}
