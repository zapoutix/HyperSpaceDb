using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace HyperSpaceDB.Models.Messages
{
    [ProtoContract]
    public class MessageHeader
    {
        [ProtoMember(1)]
        public MessageType MessageType { get; set; }
    }
}
