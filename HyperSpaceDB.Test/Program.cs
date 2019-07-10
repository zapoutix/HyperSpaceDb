using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using HyperSpaceDB.Models;
using HyperSpaceDB.Models.Messages;
using HyperSpaceDB.Test.Models;
using HyperSpaceDB.Tools.Extensions;
using ProtoBuf;

namespace HyperSpaceDB.Test
{
    class Program
    {


        static SimpleModel CreateSimpleModel()
        {
            return new SimpleModel
            {
                MyString = Guid.NewGuid().ToString(),
                MyInt = new Random().Next()
            };
        }

 
        static void Main(string[] args)
        {
            var listHosts = new List<Tuple<string, int>>();
            foreach (var arg in args)
            {
                var tab = arg.Split(":");
                listHosts.Add(new Tuple<string, int>(tab[0], Convert.ToInt32(tab[1])));
            }

            if (!listHosts.Any())
                listHosts.Add(new Tuple<string, int>("127.0.0.1", 9999));
            List<byte[]> list = new List<byte[]>();
            for (int i = 0; i < 100000; i++)
                list.Add(CreateSimpleModel().ToProtoBuf());



            Stopwatch sw = new Stopwatch();
            sw.Start();
            Random r = new Random();
            Parallel.For(0, 100, (idx) =>
            {
                try
                {
                    var host = listHosts[r.Next(0, listHosts.Count())];
                    Console.WriteLine($"Connect to {host.Item1} {host.Item2}");
                    TcpClient client = new TcpClient(host.Item1, host.Item2);
                    NetworkStream stream = client.GetStream();
                    Stopwatch sww = new Stopwatch();
                    sww.Start();
                    var transactionId = Guid.NewGuid();
                    Console.WriteLine($"[{idx}] SendCreateTransactionMessage");
                    SendCreateTransactionMessage(stream, transactionId);
                    var count = list.Count;
                    for (int o = 0; o < count; o++)
                        SendInsertMessage(stream, list[o], transactionId);

                    Console.WriteLine($"[{idx}] SendCommitTransactionMessage");
                    SendCommitTransactionMessage(stream, transactionId);
                    SendCountnMessage(stream, Guid.NewGuid());
                    stream.Flush();
                    sww.Stop();
                    Console.WriteLine("For Inserted in " + sww.Elapsed);

                    stream.Close();
                    client.Close();

                } catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });

            sw.Stop();
            Console.WriteLine("Global Inserted in " + sw.Elapsed);

        }

        static Guid SendCountnMessage(NetworkStream stream, Guid transactionKey)
        {
            MessageHeader header = new MessageHeader() { MessageType = MessageType.Count };
            header.SendToStream(stream);
            var message = new CountMessage();
            message.SendToStream(stream);

            return transactionKey;
        }

        static Guid SendCreateTransactionMessage(NetworkStream stream, Guid transactionKey)
        {
            MessageHeader header = new MessageHeader() { MessageType = MessageType.CreateTransaction };
            header.SendToStream(stream);
            var message = new CreateTransactionMessage();
            message.TransactionId = transactionKey;
            message.SendToStream(stream);

            return transactionKey;
        }

        static void SendCommitTransactionMessage(NetworkStream stream, Guid transactionKey)
        {
            MessageHeader header = new MessageHeader()
            {
                MessageType = MessageType.CommitTransaction
            };
            header.SendToStream(stream);
            var ins = new CommitTransactionMessage();
            ins.TransactionId = transactionKey;
            ins.SendToStream(stream);
        }

        static void SendInsertMessage(NetworkStream stream, byte[] proposition, Guid transactionKey)
        {
            MessageHeader header = new MessageHeader()
            {
                MessageType = MessageType.Insert
            };
            header.SendToStream(stream);
            var ins = new InsertMessage();
            ins.TransactionId = transactionKey;
            ins.Data = proposition;
            ins.SendToStream(stream);
        }
    }
}
