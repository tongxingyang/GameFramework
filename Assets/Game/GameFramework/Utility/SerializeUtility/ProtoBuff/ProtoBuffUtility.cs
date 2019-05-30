using System;
using System.IO;
using System.Text;
using GameFramework.Debug;
using UnityEngine;

namespace GameFramework.Utility.ProtoBuff
{
    public class ProtoBuffUtility
    {
        public byte[] Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.NonGeneric.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public byte[] Serialize<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(ms, obj);
                return ms.ToArray();
            }
        }
        
        public void Serialize(object obj, Stream stream)
        {
            ProtoBuf.Serializer.NonGeneric.Serialize(stream, obj);
        }
        
        public T Deserialize<T>(byte[] data)
        {
            using (var byteStream = new MemoryStream(data))
            {
                return ProtoBuf.Serializer.Deserialize<T>(byteStream);
            }
        }

        public object Deserialize(Type type, byte[] data)
        {
            using (var byteStream = new MemoryStream(data))
            {
                return ProtoBuf.Serializer.NonGeneric.Deserialize(type, byteStream);
            }
        }

        public T Deserialize<T>(byte[] buffer, int index, int count)
        {
            using (var byteStream = new MemoryStream(buffer, index, count))
            {
                return ProtoBuf.Serializer.Deserialize<T>(byteStream);
            }
        }

        public object Deserialize(Type type, byte[] buffer, int index, int count)
        {
            using (var byteStream = new MemoryStream(buffer, index, count))
            {
                return ProtoBuf.Serializer.NonGeneric.Deserialize(type, byteStream);
            }

        }

        public T Deserialize<T>(Stream stream)
        {
            return ProtoBuf.Serializer.Deserialize<T>(stream);
        }

        public object Deserialize(Type type, Stream stream)
        {
            return ProtoBuf.Serializer.NonGeneric.Deserialize(type, stream);
        }
    }
}