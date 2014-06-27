using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using RemotingObject;

namespace RemotingServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var channel = new TcpChannel(8080);
            ChannelServices.RegisterChannel(channel, false);

            //服务端激活方式：Singleton、SingalCall
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Person), "RemotingChatService", WellKnownObjectMode.Singleton);

            ////客户端激活方式
            //RemotingConfiguration.ApplicationName = "RemotingChatService";
            //RemotingConfiguration.RegisterActivatedServiceType(typeof(Person));

            Console.WriteLine("Server: running~");
            Console.ReadLine();
        }

        /// <summary>
        /// 关闭通道
        /// </summary>
        /// <param name="channelName"></param>
        private void CloseChannel(string channelName)
        {
            //获得当前已注册的通道；
            IChannel[] channels = ChannelServices.RegisteredChannels;

            //关闭指定名为MyTcp的通道；
            foreach (IChannel eachChannel in channels)
            {
                if (eachChannel.ChannelName == channelName)
                {
                    var tcpChannel = (TcpChannel)eachChannel;

                    //关闭监听；
                    tcpChannel.StopListening(null);

                    //注销通道；
                    ChannelServices.UnregisterChannel(tcpChannel);
                }
            }
        }
    }
}