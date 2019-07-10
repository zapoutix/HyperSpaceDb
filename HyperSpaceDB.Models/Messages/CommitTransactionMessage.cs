using System;
using System.Collections.Generic;
using System.Text;
using HyperSpaceDB.Models.Messages;
using ProtoBuf;

namespace HyperSpaceDB.Models
{
    [ProtoContract]
    public class CommitTransactionMessage : IMessage
    {
        public MessageType MessageType => MessageType.CommitTransaction;

        [ProtoMember(1)]
        public  Guid TransactionId { get; set; }
    }
}
