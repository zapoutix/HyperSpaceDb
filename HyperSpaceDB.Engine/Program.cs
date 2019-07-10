using System;
using System.Linq;
using System.Threading;
using HyperSpaceDB.Engine.Managers;

namespace HyperSpaceDB.Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            string database = "test";
            int port = 9999;
            if (args.Count() >= 1)
                database = args[0];
            if (args.Count() >= 2)
                port = Convert.ToInt32(args[1]);

            HyperSpaceDBManager.CreateInstance(database);
            TcpServiceManager.CreateInstance(port);
            TcpServiceManager.Instance.Start();

            Thread.Sleep(1000000);
        }
    }
}
