using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using RemotingObject;

namespace RemotingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);

            //IPerson person=new Person();
            //ObjRef objrefWellKnown = RemotingServices.Marshal(obj, "ServiceMessage");
            //RemotingServices.Marshal();

            //服务端激活方式（只能调用对象的默认构造函数，很不爽）
            IPerson person = (IPerson)Activator.GetObject(typeof(Person), "tcp://localhost:8080/RemotingChatService");

            ////客户端激活方式
            //RemotingConfiguration.RegisterActivatedClientType(typeof(Person), "tcp://localhost:8080/RemotingChatService");

            Console.WriteLine("Please enter your name: ");
            string name = Console.ReadLine();

            try
            {
                ////客户端激活方式，构造函数传入参数
                //object[] attrs = { new UrlAttribute("tcp://localhost:8080/RemotingChatService") };
                //var objs = new object[1];
                //objs[0] = name;
                //IPerson person = (Person)Activator.CreateInstance(typeof(Person), objs, attrs);

                
                person.Name = name;
                string welcome = string.Empty;
                IList<Person> all = person.Online(ref welcome);
                Console.WriteLine(welcome);
                for (int i = 0; i < all.Count; i++)
                {
                    Console.Write(all[i].Name + "\t");

                    if (i % 3 == 0)
                        Console.Write("\r\n");
                }
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }
}
