using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using MessageBusFun.Core;
using MessageBusFun.Core.Tcp;
using MessageBusFun.Core.Messages;
using System.Threading;

namespace MessageBusFun.Tests
{
    public class MessageBusTests
    {
        string _ipAddress = "127.0.0.1";
        static int _port = 100;
        int _sleep = 1000;

        [Fact]
        public void MultipleConnectionsTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(1000);
            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            Assert.Equal(2, server.ConnectedClientCount);
        }

        [Fact]
        public void RegisterOneProviderAndRequestChannelsTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port++);
            Thread.Sleep(1000);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);
            client1.RequestChannels();
            Thread.Sleep(_sleep);
            Assert.Equal("channel1", client1.ChannelListMessageClient[0]);
        }

        [Fact]
        public void RegisterTwoProvidersAndRequestChannelsTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);
            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);
            client1.RequestChannels();
            Thread.Sleep(_sleep);

            client2.RegisterProvider("channel2");
            Thread.Sleep(_sleep);
            client2.RequestChannels();
            Thread.Sleep(_sleep);
            Assert.Equal("channel2", client2.ChannelListMessageClient[1]);
        }

        [Fact]
        public void RegisterOneSubscriberAndPublishMessageTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message");
            Thread.Sleep(_sleep);
            Thread.Sleep(_sleep);
            Assert.Equal("message", client2.ChannelMessageClient[0].MessageString);
        }

        [Fact]
        public void RegisterTwoSubscribersAndPublishMessageClient1Test()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client1.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message");
            Thread.Sleep(_sleep);
            Thread.Sleep(_sleep);
            Assert.Equal("message", client1.ChannelMessageClient[0].MessageString);
        }

        [Fact]
        public void RegisterTwoSubscribersAndPublishMessageClient2Test()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client1.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message");
            Thread.Sleep(_sleep);
            Thread.Sleep(_sleep);
            Assert.Equal("message", client2.ChannelMessageClient[0].MessageString);
        }


        [Fact]
        public void RegisterOneSubscriberTwoProvidersAndPublishMessageTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client1.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);
            
            client1.PublishMessage("channel1", "message1");
            Thread.Sleep(_sleep);

            client2.PublishMessage("channel1", "message2");
            Thread.Sleep(_sleep);

            Assert.Equal(2, client1.ChannelMessageClient.Count);
            Assert.Equal("message1", client1.ChannelMessageClient[0].MessageString);
            Assert.Equal("message2", client1.ChannelMessageClient[1].MessageString);
        }


        [Fact]
        public void RegisterOneSubscriberTwoChannelsAndPublishMessageTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterProvider("channel2");
            Thread.Sleep(_sleep);

            client1.RegisterSubscriber("channel2");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message1");
            Thread.Sleep(_sleep);

            client2.PublishMessage("channel2", "message2");
            Thread.Sleep(_sleep);

            Assert.Equal(1, client1.ChannelMessageClient.Count);
            Assert.Equal("message2", client1.ChannelMessageClient[0].MessageString);
        }

        [Fact]
        public void RegisterOneSubscriberPublishMessageAndNotReceiveTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message");
            Thread.Sleep(_sleep);
            Thread.Sleep(_sleep);
            Assert.Empty(client1.ChannelMessageClient);
        }

        [Fact]
        public void RegisterAndUnregisterOneSubscriberPublishMessageAndNotReceiveTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client2.UnregisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message");
            Thread.Sleep(_sleep);

            Assert.Empty(client2.ChannelMessageClient);
        }

        [Fact]
        public void RegisterOneSubscriberUnregisterProviderPublishMessageAndNotReceiveTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client1.UnregisterProvider("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message");
            Thread.Sleep(_sleep);

            Assert.Equal("Channel Unavailable", client2.ChannelMessageClient[0].MessageString);
            Assert.Equal(1, client2.ChannelMessageClient.Count);
        }


        [Fact]
        public void ChannelUnavailableTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client1.UnregisterProvider("channel1");
            Thread.Sleep(_sleep);

            Assert.Equal("Channel Unavailable", client2.ChannelMessageClient[0].MessageString);
        }

        [Fact]
        public void RegisterOneSubscriberMultipleTimesAndPublishMessageTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message1");
            Thread.Sleep(_sleep);

            Assert.Equal("message1", client2.ChannelMessageClient[0].MessageString);
        }

        [Fact]
        public void UnregisterOneSubscriberMultipleTimesAndPublishMessageTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client2.UnregisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client2.UnregisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message1");
            Thread.Sleep(_sleep);

            Assert.Empty(client2.ChannelMessageClient);
        }

        [Fact]
        public void UnregisterOneSubscriberWithoutSubscriptionAndPublishMessageTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.UnregisterSubscriber("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message1");
            Thread.Sleep(_sleep);

            Assert.Empty(client2.ChannelMessageClient);
        }

        [Fact]
        public void UnregisterOneProviderMultipleTimesAndPublishMessageTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.UnregisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.UnregisterProvider("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message1");
            Thread.Sleep(_sleep);

            Assert.Empty(client2.ChannelMessageClient);
        }

        [Fact]
        public void UnregisterOneProviderNotRegisteredAndPublishMessageTest()
        {
            MessageBusClient.UserCount = 0;
            var server = new MessageBusServer();
            server.Start(_ipAddress, _port);

            var client1 = new MessageBusClient();
            client1.Start(_ipAddress, _port);
            Thread.Sleep(_sleep);

            var client2 = new MessageBusClient();
            client2.Start(_ipAddress, _port++);
            Thread.Sleep(_sleep);

            client1.RegisterProvider("channel1");
            Thread.Sleep(_sleep);

            client2.UnregisterProvider("channel1");
            Thread.Sleep(_sleep);

            client1.PublishMessage("channel1", "message1");
            Thread.Sleep(_sleep);

            Assert.Empty(client2.ChannelMessageClient);
        }
    }
}
