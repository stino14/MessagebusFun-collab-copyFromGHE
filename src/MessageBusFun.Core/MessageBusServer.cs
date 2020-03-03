using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageBusFun.Core.Tcp;
using MessageBusFun.Core.Database;
using MessageBusFun.Core.Messages;

namespace MessageBusFun.Core
{
    public class MessageBusServer : IMessageBusServer
    {
        public int ConnectedClientCount => _tcpServer.ConnectedClientCount;
        private MesageBusTcpServer _tcpServer = new MesageBusTcpServer();
        private List<Channel> _channels; //Todo: write to db
        private List<Provider> _providers; //Todo: write to db
        private List<Subscriber> _subscribers; //Todo: write to db
        private List<User> _users; 
        private Queue<Database.Message> _sendMessageQueue = new Queue<Database.Message>();
        private int _channelID = 0;
        private int _providerID = 0;
        private int _subscriberID = 0;
        private int _messageID = 0;

        public MessageBusServer()
        {

        }

        public void Start(string ipAddress, int port)
        {
            _tcpServer.Start(ipAddress, port);
            _users = LoadUsers();
            _subscribers = LoadSubscribers();
            _providers = LoadProviders();
            _channels = LoadChannels();
            _tcpServer.DataReceived += DataReceived;
        }

        private List<Channel> LoadChannels()
        {
            return new List<Channel>();
        }

        private List<Provider> LoadProviders()
        {
            return new List<Provider>();
        }

        private List<Subscriber> LoadSubscribers()
        {
            return new List<Subscriber>();
        }

        //Todo: read from db
        private List<User> LoadUsers()
        {
            return new List<User>() {
                new User() { Id = 0, UserName = "user0", Password = "pwd" },
                new User() { Id = 1, UserName = "user1", Password = "pwd" },
                new User() { Id = 2, UserName = "user2", Password = "pwd" }
            };
        }

        //Todo: refactor to separate methods
        private void DataReceived(object sender, TcpClientData tcpClientData)
        {
            var message = MessageFactory.Create(tcpClientData.MessageBytes);

            if (ClientAuthenticated(message))
            {
                if (message.MessageType == MessageType.Registration)
                {
                    var registrationMessage = (RegistrationMessage)message;

                    var channel = _channels.Where(p => p.Name == registrationMessage.Channel).FirstOrDefault();
                    if (registrationMessage.ClientType == ClientType.Provider)
                    {
                        var provider = _providers.Where(p => p.Username == registrationMessage.User).FirstOrDefault();
                        if (registrationMessage.RegistrationType == RegistrationType.Register)
                        {
                            if (channel == null)
                            {
                                channel = new Channel() { Name = registrationMessage.Channel, ProviderCount = 1 };
                                channel.Id = _channelID++;
                                _channels.Add(channel);
                                Console.WriteLine("Channel added: {0} by: {1}", registrationMessage.Channel, registrationMessage.User);
                            }
                            else
                            {
                                channel.ProviderCount++;
                            }

                            if (provider == null)
                            {
                                provider = new Provider() { Username = registrationMessage.User };
                                provider.Id = _providerID++;
                                provider.ClientId = tcpClientData.ClientID;
                                _providers.Add(provider);
                                Console.WriteLine("Provider registered: {0} for: {1}", registrationMessage.User, registrationMessage.Channel);
                            }
                        }
                        else
                        {
                            if (channel != null)
                            {
                                channel.ProviderCount--;
                                if (channel.ProviderCount == 0)
                                {
                                    var channelUnavailableMessage = new ChannelUnavailableMessage() { Channel = channel.Name };
                                    foreach (var subscriber in _subscribers)
                                    {
                                        var msg = new Database.Message();
                                        msg.Id = _messageID++;
                                        msg.SubscriberId = subscriber.Id;
                                        msg.ByteMessageString = Encoding.ASCII.GetString(channelUnavailableMessage.ToByteArray());
                                        msg.ChannelId = channel.Id;
                                        msg.Channel = channel.Name;
                                        msg.ClientId = subscriber.ClientId;
                                        _sendMessageQueue.Enqueue(msg);
                                        SendMessages();
                                    }
                                }
                            }

                            if (provider != null)
                            {
                                _providers.Remove(provider);
                                Console.WriteLine("Provider unregistered: {0} for: {1}", registrationMessage.User, registrationMessage.Channel);
                            }
                        }
                    }
                    else if (registrationMessage.ClientType == ClientType.Subscriber)
                    {
                        var subscriber = _subscribers.Where(p => p.Username == registrationMessage.User).FirstOrDefault();
                        if (registrationMessage.RegistrationType == RegistrationType.Register)
                        {
                            if (subscriber == null)
                            {
                                subscriber = new Subscriber() { Username = registrationMessage.User };
                                subscriber.Id = _subscriberID++;
                                subscriber.ClientId = tcpClientData.ClientID;
                                subscriber.Channel = channel.Name;
                                _subscribers.Add(subscriber);
                                Console.WriteLine("Subscriber registered: {0} for: {1}", registrationMessage.User, registrationMessage.Channel);
                            }
                        }
                        else
                        {
                            if (subscriber != null)
                            {
                                _subscribers.Remove(subscriber);
                                Console.WriteLine("Subscriber unregistered: {0} for: {1}", registrationMessage.User, registrationMessage.Channel);
                            }
                        }
                    }
                }

                if (message.MessageType == MessageType.Channel)
                {
                    var channelMessage = (ChannelMessage)message;
                    var channel = _channels.Where(p => p.Name == channelMessage.Channel).FirstOrDefault();

                    if (_providers.FirstOrDefault(p => p.ClientId == tcpClientData.ClientID) != null)
                    {
                        foreach (var subscriber in _subscribers)
                        {
                            if (subscriber.Channel == channel.Name)
                            {
                                var msg = new Database.Message();
                                msg.Id = _messageID++;
                                msg.SubscriberId = subscriber.Id;
                                msg.ByteMessageString = Encoding.ASCII.GetString(channelMessage.ToByteArray());
                                msg.ChannelId = channel.Id;
                                msg.Channel = channel.Name;
                                msg.ClientId = subscriber.ClientId;
                                _sendMessageQueue.Enqueue(msg);
                            }
                        }

                        SendMessages();
                    }
                }

                if (message.MessageType == MessageType.GetChannels)
                {
                    var requestChannelsMessage = (RequestChannelsMessage)message;

                    requestChannelsMessage.Channels = _channels.Select(p => p.Name).ToList();
                    var msg = new Database.Message();
                    msg.Id = _messageID++;
                    msg.ByteMessageString = Encoding.ASCII.GetString(requestChannelsMessage.ToByteArray());
                    msg.ClientId = tcpClientData.ClientID;
                    _sendMessageQueue.Enqueue(msg);
                    SendMessages();
                }
            }
        }

        private bool ClientAuthenticated(Messages.Message message)
        {
            var user = new User() { UserName = message.User, Password = message.Password };
            return _users.FirstOrDefault(p => p.UserName == message.User && p.Password == message.Password) != null;
        }

        private void SendMessages()
        {
            //Todo: change to using an async callback to confirm send and remove from queue and DB; use subscriber id instead of client id to handle disconnects
            while (_sendMessageQueue.Count > 0)
            {
                var message = _sendMessageQueue.Dequeue();
                _tcpServer.SendMessage(Encoding.ASCII.GetBytes(message.ByteMessageString), message.ClientId);
            }
        }
    }
}
