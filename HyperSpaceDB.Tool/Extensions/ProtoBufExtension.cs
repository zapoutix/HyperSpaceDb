using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using ProtoBuf.Meta;

namespace HyperSpaceDB.Tools.Extensions
{
    public static class ProtoBufExtension
    {
        public static byte[] ToProtoBuf(this object obj)
        {
            RuntimeTypeModel.Default.MetadataTimeoutMilliseconds = 15000;
            using (var m = new MemoryStream())
            {
                
                Serializer.SerializeWithLengthPrefix(m, obj, PrefixStyle.Base128);
                m.Position = 0;
                return m.ToArray();
            }
        }
        static object lockobj = new object();
        public static void SendToStream<Tobj>(this Tobj obj, Stream stream)
        {
            Serializer.SerializeWithLengthPrefix(stream, obj, PrefixStyle.Base128);
        }
    }
}
