using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace HyperSpaceDB.Models.Messages
{
    [ProtoContract]
    public enum MessageType
    {
        [ProtoEnum]
        Insert,
        [ProtoEnum]
        Delete,
        [ProtoEnum]
        CreateTransaction,
        [ProtoEnum]
        CommitTransaction,
        [ProtoEnum]
        RollbackTransaction,
        [ProtoEnum]
        Count
    }
}
