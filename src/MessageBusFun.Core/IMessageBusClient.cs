namespace MessageBusFun.Core
{
    public interface IMessageBusClient
    {
        void Start(string ipAddress, int port);
        void RegisterProvider(string channel);
        void UnregisterProvider(string channel);
        void RegisterSubscriber(string channel);
        void UnregisterSubscriber(string channel);
        void PublishMessage(string channel, string messageString);
        void RequestChannels();
    }
}