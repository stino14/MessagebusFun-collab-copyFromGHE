namespace MessageBusFun.Core
{
    public interface IMessageBusServer
    {
        void Start(string ipAddress, int port);
    }
}