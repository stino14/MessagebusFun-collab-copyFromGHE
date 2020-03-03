using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using MessageBusFun.Core.Messages;

namespace MessageBusFun.Tests
{
    public class MessageTests
    {
        [Fact]
        public void ChannelMessageTest()
        {
            var msg = MessageType.Channel.ToString() + "|UserName|pwd|TestChannel|Test";
            var messageBytes = Encoding.ASCII.GetBytes(msg + Message.EOF);
            var message = (ChannelMessage)MessageFactory.Create(messageBytes);
            Assert.Equal("TestChannel", message.Channel);
            Assert.Equal("UserName", message.User);
            Assert.Equal("pwd", message.Password);
            Assert.Equal("Test", message.MessageString);
            Assert.Equal(msg, message.ToString());
            Assert.Equal(messageBytes, message.ToByteArray());
        }

        [Fact]
        public void ChannelUnavailableMessageTest()
        {
            var channelUnavailableMessage = new ChannelUnavailableMessage() { Channel = "TestChannel" };
            var msg = MessageType.ChannelUnavailable.ToString() + "|UserName|pwd|TestChannel|Channel Unavailable";
            var messageBytes = Encoding.ASCII.GetBytes(msg + Message.EOF);
            var message = (ChannelUnavailableMessage)MessageFactory.Create(messageBytes);
            Assert.Equal("TestChannel", message.Channel);
            Assert.Equal("UserName", message.User);
            Assert.Equal("pwd", message.Password);
            Assert.Equal("Channel Unavailable", message.MessageString);
            Assert.Equal(msg, message.ToString());
            Assert.Equal(messageBytes, message.ToByteArray());
        }

        //[Fact]
        //public void RegisterProviderMessageTest()
        //{
        //    var msg = MessageType.RegisterProvider.ToString() + "|UserName|pwd|TestChannel";
        //    var messageBytes = Encoding.ASCII.GetBytes(msg + Message.EOF);
        //    var message = (RegisterProviderMessage)MessageFactory.Create(messageBytes);
        //    Assert.Equal("TestChannel", message.Channel);
        //    Assert.Equal("UserName", message.User);
        //    Assert.Equal("pwd", message.Password);
        //    Assert.Equal(msg, message.ToString());
        //    Assert.Equal(messageBytes, message.ToByteArray());
        //}

        //[Fact]
        //public void RegisterSubscriberMessageTest()
        //{
        //    var msg = MessageType.RegisterSubscriber.ToString() + "|UserName|pwd|TestChannel";
        //    var messageBytes = Encoding.ASCII.GetBytes(msg + Message.EOF);
        //    var message = (RegisterSubscriberMessage)MessageFactory.Create(messageBytes);
        //    Assert.Equal("TestChannel", message.Channel);
        //    Assert.Equal("UserName", message.User);
        //    Assert.Equal("pwd", message.Password);
        //    Assert.Equal(msg, message.ToString());
        //    Assert.Equal(messageBytes, message.ToByteArray());
        //}

        [Fact]
        public void RegistrationMessageTest()
        {
            var msg = MessageType.Registration.ToString() + "|UserName|pwd|" + ClientType.Provider.ToString() + "|" + RegistrationType.Register + "|" + "TestChannel";
            var messageBytes = Encoding.ASCII.GetBytes(msg + Message.EOF);
            var message = (RegistrationMessage)MessageFactory.Create(messageBytes);
            Assert.Equal("TestChannel", message.Channel);
            Assert.Equal("UserName", message.User);
            Assert.Equal("pwd", message.Password);
            Assert.Equal(msg, message.ToString());
            Assert.Equal(messageBytes, message.ToByteArray());
        }

        [Fact]
        public void GetChannelsMessageTest()
        {
            var msg = MessageType.GetChannels.ToString() + "|UserName|pwd|channel1;channel2";
            var messageBytes = Encoding.ASCII.GetBytes(msg + Message.EOF);
            var message = (RequestChannelsMessage)MessageFactory.Create(messageBytes);
            Assert.Equal("UserName", message.User);
            Assert.Equal("pwd", message.Password);
            Assert.Equal("channel2", message.Channels[1]);
            Assert.Equal(msg, message.ToString());
            Assert.Equal(messageBytes, message.ToByteArray());
        }
    }
}
