using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using HyperSpaceDB.Models;
using HyperSpaceDB.Models.Messages;
using ProtoBuf;
using ProtoBuf.Meta;

namespace HyperSpaceDB.Engine.Managers
{
    class TcpServiceManager
    {
        static TcpServiceManager _instance;
        public static TcpServiceManager Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("You should call CreateInstance first");

                return _instance;
            }
        }
        public static void CreateInstance(int port)
        {
            _instance = new TcpServiceManager(port);
        }


        int _port;

        public TcpServiceManager(int port)
        {
            _port = port;
        }

        public void Start()
        {
            IPAddress ipAddress = IPAddress.Any;
            Console.WriteLine("Starting TCP listener...");
            TcpListener listener = new TcpListener(ipAddress, _port);
            listener.Start();
            Serializer.PrepareSerializer<MessageHeader>();
            Serializer.PrepareSerializer<InsertMessage>();
            Serializer.PrepareSerializer<CreateTransactionMessage>();
            Serializer.PrepareSerializer<CommitTransactionMessage>();
            var type = this.GetType();
            RuntimeTypeModel.Default.Add(type, true);
            Int32 i = 1;
            foreach (PropertyInfo info in type.GetProperties())
            {
                if (info.CanWrite)
                {
                    RuntimeTypeModel.Default[type].AddField(i++, info.Name);
                }
            }

            while (true) // <--- boolean flag to exit loop
            {
                if (listener.Pending())
                {
                    var clt = listener.AcceptTcpClient();

                    Thread newThread = new Thread(OnNewConnection);
                    newThread.Start(clt);
                }
                else
                {
                    Thread.Sleep(100); //<--- timeout
                }
            }
        }

        private void OnNewConnection(object obj)
        {
            try
            {
                Console.WriteLine("Get New Connection");
                bool isRunnuing = true;
                TcpClient client = (TcpClient)obj;
                var id = Thread.CurrentThread.ManagedThreadId;
                using (NetworkStream ns = client.GetStream())
                {
                    while (isRunnuing)
                    {
                        //Get MessageHeader
                        MessageHeader header = null;
                        try
                        {
                            header = Serializer.DeserializeWithLengthPrefix<MessageHeader>(ns, PrefixStyle.Base128);
                            if (header == null)
                            {
                                client.Close();
                                isRunnuing = false;
                                Console.WriteLine($"[{id}] Close connection to client");
                                Thread.Sleep(100);
                                continue;
                            }
                        } catch (Exception e)
                        {
                            Console.WriteLine($"[{id}] Error get header");
                            continue;
                        }

                        //Console.WriteLine($"[{id}] Get header: " + header.MessageType.ToString());
                        IMessage messageRequest = null;
                        IMessage messageResponse = null;
                        if (header.MessageType == MessageType.Insert)
                        {
                            //Console.WriteLine($"[{id}] Insert");
                            messageRequest = Serializer.DeserializeWithLengthPrefix<InsertMessage>(ns, PrefixStyle.Base128);
                        }
                        if (header.MessageType == MessageType.CreateTransaction)
                        {
                            Console.WriteLine($"[{id}] CreateTransaction");
                            messageRequest = Serializer.DeserializeWithLengthPrefix<CreateTransactionMessage>(ns, PrefixStyle.Base128);
                        }
                        if (header.MessageType == MessageType.CommitTransaction)
                        {
                            Console.WriteLine($"[{id}] CommitTransaction");
                            messageRequest = Serializer.DeserializeWithLengthPrefix<CommitTransactionMessage>(ns, PrefixStyle.Base128);
                        }
                        if (header.MessageType == MessageType.Count)
                        {
                            Console.WriteLine($"[{id}] Count");
                            messageRequest = Serializer.DeserializeWithLengthPrefix<CountMessage>(ns, PrefixStyle.Base128);
                        }
                        if (messageRequest != null)
                            messageResponse = MessageService.ProcessMessage(messageRequest);

                        //Handle response
                        if (messageResponse != null)
                        {

                        }
                    }

                }
            }catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
