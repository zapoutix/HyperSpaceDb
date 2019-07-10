using System;
using System.Collections.Generic;
using System.Text;

namespace HyperSpaceDB.Models.Messages
{
    public interface IMessage
    {
        MessageType MessageType { get; }
    }
}
