using System;
using System.Collections.Generic;
using System.Text;

namespace RemotingObject
{
    public interface IPerson
    {
        string Name { get; set; }
        IList<Person> Online(ref string welcome);
        string Offline();
    }

    [Serializable]
    public class Person : MarshalByRefObject, IPerson
    {
        public string Name { get; set; }

        /// <summary>
        /// 无法直接在构造函数中指定name很不爽，因为使用了客户端激活方式
        /// 若使用构造函数传参，则每个客户端是一个实例，无法产生互通（无法维持客户端list）
        /// </summary>
        public Person()
        {
            Console.WriteLine(string.Format("[{0}]:Remoting Object '{0}' is activated.", typeof(Person).Name));
        }

        /// <summary>
        /// 所有人员列表
        /// </summary>
        private IList<Person> All = new List<Person>();

        /// <summary>
        /// 上线方法
        /// </summary>
        /// <param name="welcome"></param>
        /// <returns></returns>
        public IList<Person> Online(ref string welcome)
        {
            if (string.IsNullOrEmpty(Name))
                throw new Exception("Invalid name");

            bool exists = false;
            foreach (var i in All)
            {
                if (i.Name == Name)
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                Console.WriteLine(string.Format("[back online]: '{0}' ", Name));
                welcome = "Welcome back: " + Name;
            }
            else
            {
                All.Add(this);
                Console.WriteLine(string.Format("[user online]: '{0}'", Name));
                welcome = "Welcome: " + Name;
            }

            return All;
        }

        /// <summary>
        /// 离线方法
        /// </summary>
        /// <returns></returns>
        public string Offline()
        {
            Console.WriteLine(Name + " is offline");
            return "Bye bye~ " + Name;
        }

        /// <summary>
        /// 无限生命周期
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }

    public interface IMessage
    {
        void Send();
        Message Receive(Person me);
    }

    public class Message : MarshalByRefObject, IMessage
    {
        public Person MessageFrom { get; set; }
        public Person MessageTo { get; set; }
        public string MessageContent { get; set; }
        public DateTime CreateTime { get; set; }

        public Message(Person from, Person to, string content)
        {
            MessageFrom = from;
            MessageTo = to;
            MessageContent = content;
            CreateTime = DateTime.Now;
        }

        private IList<Message> All { get; set; }

        /// <summary>
        /// 发送信息（加入信息列表）
        /// </summary>
        public void Send()
        {
            All.Add(new Message(MessageFrom, MessageTo, MessageContent));
        }

        /// <summary>
        /// 接收信息
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public Message Receive(Person me)
        {
            foreach (Message m in All)
            {
                if (m.MessageTo.Name == me.Name)
                {
                    return m;
                }
            }
            return null;
        }
    }
}
