using System;
using System.Collections.Generic;
using System.Text;
using HyperSpaceDB.Models;
using HyperSpaceDB.Models.Messages;

namespace HyperSpaceDB.Engine.Managers
{
    public static class MessageService
    {
        public static IMessage ProcessMessage(IMessage message)
        {
            if (message.MessageType == MessageType.Insert)
            {
                var msg = (InsertMessage)message;
                HyperSpaceDBManager.Instance.Insert(msg.TransactionId, msg.Data);

                return null;
            }

            if (message.MessageType == MessageType.CreateTransaction)
            {
                var msg = (CreateTransactionMessage)message;
                HyperSpaceDBManager.Instance.CreateTransaction(msg.TransactionId);

                return null;
            }

            if (message.MessageType == MessageType.CommitTransaction)
            {
                var msg = (CommitTransactionMessage)message;
                HyperSpaceDBManager.Instance.CommitTransaction(msg.TransactionId);

                return null;
            }

            if (message.MessageType == MessageType.Count)
            {
                var msg = (CountMessage)message;
                var count  = HyperSpaceDBManager.Instance.Count("Propositions");
                Console.WriteLine("Count : " + count);
                return null;
            }
            return null;
        }
    }
}
