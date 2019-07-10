using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace HyperSpaceDB.Models.Messages
{
    [ProtoContract]
    public class InsertMessage : IMessage
    {
        public MessageType MessageType => MessageType.Insert;

        [ProtoMember(1)]
        public Guid TransactionId { get; set; }

        [ProtoMember(2)]
        public byte[] Data { get; set; }

    }
}
