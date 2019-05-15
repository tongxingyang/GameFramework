using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace GameFramework.Utility
{
    public class ByteBuffer : IDisposable
    {
        MemoryStream stream = null;
        BinaryWriter writer = null;
        BinaryReader reader = null;

        public ByteBuffer()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        public ByteBuffer(byte[] data)
        {
            if (data != null)
            {
                stream = new MemoryStream(data);
                reader = new BinaryReader(stream);
            }
        }

        public void InitWithBytes(byte[] v)
        {
            writer.Write(v);
        }
        
        public void Dispose()
        {
            Close();
        }
        
        public void Close()
        {
            writer?.Close();
            reader?.Close();

            stream.Close();
            writer = null;
            reader = null;
            stream = null;
        }

        public void WriteByte(byte v)
        {
            writer.Write(v);
        }
        
        public void WriteSByte(sbyte v)
        {
            writer.Write(v);
        }

        public void WriteBool(bool v)
        {
            writer.Write(v);
        }
        
        public void WriteChar(char v)
        {
            writer.Write(v);
        }
        
        public void WriteShort(short v)
        {
            writer.Write(v);
        }
        
        public void WriteUShort(ushort v)
        {
            writer.Write(v);
        }
        
        public void WriteInt(int v)
        {
            writer.Write(v);
        }
        
        public void WriteUInt(uint v)
        {
            writer.Write(v);
        }

        public void WriteLong(long v)
        {
            writer.Write(v);
        }

        public void WriteULong(ulong v)
        {
            writer.Write(v);
        }
        
        public void WriteFloat(float v)
        {
            byte[] temp = BitConverter.GetBytes(v);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToSingle(temp, 0));
        }
        
        public void WriteDouble(double v)
        {
            byte[] temp = BitConverter.GetBytes(v);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToDouble(temp, 0));
        }
        
        public void WriteVector2(Vector2 v)
        {
            WriteFloat(v.x);
            WriteFloat(v.y);
        }

        public void WriteVector3(Vector3 v)
        {
            WriteFloat(v.x);
            WriteFloat(v.y);
            WriteFloat(v.z);
        }
        
        public void WriteVector4(Vector4 v)
        {
            WriteFloat(v.x);
            WriteFloat(v.y);
            WriteFloat(v.z);
            WriteFloat(v.w);
        }

        public void WriteColor32(Color32 v)
        {
            WriteByte(v.r);
            WriteByte(v.g);
            WriteByte(v.b);
            WriteByte(v.a);
        }

        public void WriteColor(Color v)
        {
            WriteFloat(v.r);
            WriteFloat(v.g);
            WriteFloat(v.b);
            WriteFloat(v.a);
        }

        public void WriteDataTime(DateTime v)
        {
            TimeSpan span = ( v - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());  
            WriteLong(span.Ticks);
        }

        public void WriteRect(Rect v)
        {
            WriteFloat(v.x);
            WriteFloat(v.y);
            WriteFloat(v.width);
            WriteFloat(v.height);
        }

        public void WriteQuaternion(Quaternion v)
        {
            WriteFloat(v.x);
            WriteFloat(v.y);
            WriteFloat(v.z);
            WriteFloat(v.w);
        }
        
        public void WriteString(string v)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            writer.Write((ushort)bytes.Length);
            writer.Write(bytes);
        }
        
        public void WriteBytes(byte[] v)
        {
            writer.Write((ushort) v.Length);
            writer.Write(v);
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }
        
        public sbyte ReadSByte()
        {
            return reader.ReadSByte();
        }

        public bool ReadBool()
        {
            return reader.ReadBoolean();
        }
        
        public char ReadChar()
        {
            return reader.ReadChar();
        }
        
        public short ReadShort()
        {
            return reader.ReadInt16();
        }
        
        public ushort ReadUShort()
        {
            return (ushort) reader.ReadInt16();
        }
        
        public int ReadInt()
        {
            return reader.ReadInt32();
        }
               
        public uint ReadUInt()
        {
            return (uint) reader.ReadInt32();
        }

        public long ReadLong()
        {
            return reader.ReadInt64();
        }

        public ulong ReadULong()
        {
            return (ulong)reader.ReadInt64();
        }
        
        public float ReadFloat()
        {
            byte[] temp = BitConverter.GetBytes(reader.ReadSingle());
            Array.Reverse(temp);
            return BitConverter.ToSingle(temp, 0);
        }

        public double ReadDouble()
        {
            byte[] temp = BitConverter.GetBytes(reader.ReadDouble());
            Array.Reverse(temp);
            return BitConverter.ToDouble(temp, 0);
        }

        public Vector2 ReadVector2()
        {
            return new Vector2(ReadFloat(),ReadFloat());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadFloat(),ReadFloat(),ReadFloat());
        }
        
        public Vector4 ReadVector4()
        {
            return new Vector4(ReadFloat(),ReadFloat(),ReadFloat(),ReadFloat());
        }

        public Color32 ReadColor32()
        {
            return new Color32(ReadByte(),ReadByte(),ReadByte(),ReadByte());
        }

        public Color ReadColor()
        {
            return new Color(ReadFloat(),ReadFloat(),ReadFloat(),ReadFloat());
        }

        public DateTime ReadDataTime()
        {
            return new DateTime(ReadLong());
        }

        public Rect ReadRect()
        {
            return new Rect(ReadFloat(),ReadFloat(),ReadFloat(),ReadFloat());
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(ReadFloat(),ReadFloat(),ReadFloat(),ReadFloat());
        }
        
        public string ReadString()
        {
            ushort len = ReadUShort();
            var temp = reader.ReadBytes(len);
            return Encoding.UTF8.GetString(temp);
        }

        public byte[] ReadBytes()
        {
            ushort len = ReadUShort();
            return reader.ReadBytes(len);
        }

        public byte[] ToBytes()
        {
            writer?.Flush();
            return stream.ToArray();
        }

        public void Flush()
        {
            writer.Flush();
        }

        public long Seek(int offect,SeekOrigin seekOrigin)
        {
            return writer.Seek(offect, seekOrigin);
        }
    }
}