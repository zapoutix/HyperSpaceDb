using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace HyperSpaceDB.Models.Messages
{
    [ProtoContract]
    public class CountMessage : IMessage
    {
        public MessageType MessageType => MessageType.Count;
 
    }
}
