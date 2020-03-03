using System;
using System.Linq;
using Xunit;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MessageBusFun.Core.Messages;
using MessageBusFun.Core.Tcp;

namespace MessageBusFun.Core.Tests
{
    public class TcpTests
    {
        string _ipAddress = "127.0.0.1";
        static int _port = 1000;
        string _certificateFilename = @"c:\temp\OpenSSL\server.crt";
        int _sleep = 1000;

        [Fact]
        public void TcpServerSingleConnectionTest()
        {
            var server = new MesageBusTcpServer();
            server.Start(_ipAddress, _port, _certificateFilename);
            var tcpClient = new TcpClient();
            tcpClient.Connect(_ipAddress, _port++);
            Thread.Sleep(_sleep);
            Assert.Equal(1, server.ConnectedClientCount);
        }

        [Fact]
        public void TcpServerMultipleConnectionTest()
        {
            var server = new MesageBusTcpServer();
            server.Start(_ipAddress, _port, _certificateFilename);
            var tcpClient1 = new TcpClient();
            tcpClient1.Connect(_ipAddress, _port);
            var tcpClient2 = new TcpClient();
            tcpClient2.Connect(_ipAddress, _port++);
            Thread.Sleep(_sleep);
            Assert.Equal(2, server.ConnectedClientCount);
        }

        //[Fact(Skip = "Temporary")]
        //public void TcpClientWriteTest()
        //{
        //    var server = new MesageBusFunSslTcpServer();
        //    server.Start(_ipAddress, _port, _certificateFilename);
        //    var client = new MessageBusFunSslTcpClient();
        //    client.Start(_ipAddress, _port++);
        //    var message = new ChannelMessage() { Channel = "TestChannel", MessageString = "Test" };
        //    client.SendMessage(message);
        //    Thread.Sleep(_sleep);
        //    Assert.Equal(message.ToString(), server.Message.ToString());
        //    server.SendMessage(message, 0);
        //    Thread.Sleep(_sleep);
        //    Assert.Equal(message.ToString(), client.Message.ToString());
        //}


        //[Fact(Skip = "Temporary")]
        //public void TcpClientWriteBytesTest()
        //{
        //    var server = new MesageBusFunSslTcpServer();
        //    server.Start(_ipAddress, _port, _certificateFilename);
        //    var client = new MessageBusFunSslTcpClient();
        //    client.Start(_ipAddress, _port++);
        //    var message = new ChannelMessage() { Channel = "TestChannel", MessageString = "Test" };
        //    client.SendMessage(message.ToByteArray());
        //    Thread.Sleep(_sleep);
        //    Assert.Equal(message.ToString(), server.Message.ToString());
        //    server.SendMessage(message, 0);
        //    Thread.Sleep(_sleep);
        //    Assert.Equal(message.ToString(), client.Message.ToString());
        //    var client2 = new MessageBusFunSslTcpClient();
        //    client.Start(_ipAddress, _port);
        //}
    }
}
