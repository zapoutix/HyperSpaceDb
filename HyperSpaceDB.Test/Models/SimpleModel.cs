using System;
using ProtoBuf;

namespace HyperSpaceDB.Test.Models
{
    [ProtoContract]
    public class SimpleModel
    {
        [ProtoMember(1)]
        public string MyString;
        [ProtoMember(2)]
        public int MyInt;
    }

}
