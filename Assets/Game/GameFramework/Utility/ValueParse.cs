using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Utility;
using UnityEngine;

namespace GameFramework.Utility
{

    public abstract class ValueParseBase<T>
    {
        public abstract Type Type { get; }
        public abstract void ReadValue(ByteBuffer buffer, ref T t);
        public abstract void WriteValue(ByteBuffer buffer, T t);
    }

    public sealed class ByteParse : ValueParseBase<byte>
    {
        public override Type Type => typeof(byte);
        public override void ReadValue(ByteBuffer buffer, ref byte t)
        {
            t = buffer.ReadByte();
        }

        public override void WriteValue(ByteBuffer buffer, byte t)
        {
            buffer.WriteByte(t);
        }
    }
    
    public sealed class SByteParse : ValueParseBase<sbyte>
    {
        public override Type Type => typeof(sbyte);
        public override void ReadValue(ByteBuffer buffer, ref sbyte t)
        {
            t = buffer.ReadSByte();
        }

        public override void WriteValue(ByteBuffer buffer, sbyte t)
        {
            buffer.WriteSByte(t);
        }
    }

    public sealed class BoolParse : ValueParseBase<bool>
    {
        public override Type Type => typeof(bool);
        public override void ReadValue(ByteBuffer buffer, ref bool t)
        {
            t = buffer.ReadBool();
        }

        public override void WriteValue(ByteBuffer buffer, bool t)
        {
            buffer.WriteBool(t);
        }
    }

    public sealed class CharParse : ValueParseBase<char>
    {
        public override Type Type => typeof(char);
        public override void ReadValue(ByteBuffer buffer, ref char t)
        {
            t = buffer.ReadChar();
        }

        public override void WriteValue(ByteBuffer buffer, char t)
        {
            buffer.WriteChar(t);
        }
    }
    
    public sealed class ShortParse : ValueParseBase<short>
    {
        public override Type Type => typeof(short);
        public override void ReadValue(ByteBuffer buffer, ref short t)
        {
            t = buffer.ReadShort();
        }

        public override void WriteValue(ByteBuffer buffer, short t)
        {
            buffer.WriteShort(t);
        }
    }
    
    public sealed class UShortParse : ValueParseBase<ushort>
    {
        public override Type Type => typeof(ushort);
        public override void ReadValue(ByteBuffer buffer, ref ushort t)
        {
            t = buffer.ReadUShort();
        }

        public override void WriteValue(ByteBuffer buffer, ushort t)
        {
            buffer.WriteUShort(t);
        }
    }
    
    public sealed class UIntParse : ValueParseBase<uint>
    {
        public override Type Type => typeof(uint);
        public override void ReadValue(ByteBuffer buffer, ref uint t)
        {
            t = buffer.ReadUInt();
        }

        public override void WriteValue(ByteBuffer buffer, uint t)
        {
            buffer.WriteUInt(t);
        }
    }
    
    public sealed class IntParse : ValueParseBase<int>
    {
        public override Type Type => typeof(int);
        public override void ReadValue(ByteBuffer buffer, ref int t)
        {
            t = buffer.ReadInt();
        }

        public override void WriteValue(ByteBuffer buffer, int t)
        {
            buffer.WriteInt(t);
        }
    }

    public sealed class LongParse : ValueParseBase<long>
    {
        public override Type Type => typeof(long);
        public override void ReadValue(ByteBuffer buffer, ref long t)
        {
            t = buffer.ReadLong();
        }

        public override void WriteValue(ByteBuffer buffer, long t)
        {
            buffer.WriteLong(t);
        }
    }
    
    public sealed class ULongParse : ValueParseBase<ulong>
    {
        public override Type Type => typeof(ulong);
        public override void ReadValue(ByteBuffer buffer, ref ulong t)
        {
            t = buffer.ReadULong();
        }

        public override void WriteValue(ByteBuffer buffer, ulong t)
        {
            buffer.WriteULong(t);
        }
    }

    public sealed class FloatParse : ValueParseBase<float>
    {
        public override Type Type => typeof(float);
        public override void ReadValue(ByteBuffer buffer, ref float t)
        {
            t = buffer.ReadFloat();
        }

        public override void WriteValue(ByteBuffer buffer, float t)
        {
            buffer.WriteFloat(t);
        }
    }

    public sealed class DoubleParse : ValueParseBase<double>
    {
        public override Type Type => typeof(double);
        public override void ReadValue(ByteBuffer buffer, ref double t)
        {
            t = buffer.ReadDouble();
        }

        public override void WriteValue(ByteBuffer buffer, double t)
        {
            buffer.WriteDouble(t);
        }
    }

    public sealed class StringParse : ValueParseBase<string>
    {
        public override Type Type => typeof(string);
        public override void ReadValue(ByteBuffer buffer, ref string t)
        {
            t = buffer.ReadString();
        }

        public override void WriteValue(ByteBuffer buffer, string t)
        {
            buffer.WriteString(t);
        }
    }

    public sealed class Vector2Parse : ValueParseBase<Vector2>
    {
        public override Type Type => typeof(Vector2);
        public override void ReadValue(ByteBuffer buffer, ref Vector2 t)
        {
            t = buffer.ReadVector2();
        }

        public override void WriteValue(ByteBuffer buffer, Vector2 t)
        {
            buffer.WriteVector2(t);
        }
    }
    
    public sealed class Vector3Parse : ValueParseBase<Vector3>
    {
        public override Type Type => typeof(Vector3);
        public override void ReadValue(ByteBuffer buffer, ref Vector3 t)
        {
            t = buffer.ReadVector3();
        }

        public override void WriteValue(ByteBuffer buffer, Vector3 t)
        {
            buffer.WriteVector3(t);
        }
    }    
    
    public sealed class Vector4Parse : ValueParseBase<Vector4>
    {
        public override Type Type => typeof(Vector4);
        public override void ReadValue(ByteBuffer buffer, ref Vector4 t)
        {
            t = buffer.ReadVector4();
        }

        public override void WriteValue(ByteBuffer buffer, Vector4 t)
        {
            buffer.WriteVector4(t);
        }
    }

    public sealed class Color32Parse : ValueParseBase<Color32>
    {
        public override Type Type => typeof(Color32);
        public override void ReadValue(ByteBuffer buffer, ref Color32 t)
        {
            t = buffer.ReadColor32();
        }

        public override void WriteValue(ByteBuffer buffer, Color32 t)
        {
            buffer.WriteColor32(t);
        }
    }

    public sealed class ColorParse : ValueParseBase<Color>
    {
        public override Type Type => typeof(Color);
        public override void ReadValue(ByteBuffer buffer, ref Color t)
        {
            t = buffer.ReadColor();
        }

        public override void WriteValue(ByteBuffer buffer, Color t)
        {
            buffer.WriteColor(t);
        }
    }

    public sealed class DateTimeParse : ValueParseBase<DateTime>
    {
        public override Type Type => typeof(DateTime);
        public override void ReadValue(ByteBuffer buffer, ref DateTime t)
        {
            t = buffer.ReadDataTime();
        }

        public override void WriteValue(ByteBuffer buffer, DateTime t)
        {
            buffer.WriteDataTime(t);
        }
    }

    public sealed class RectParse : ValueParseBase<Rect>
    {
        public override Type Type => typeof(Rect);
        public override void ReadValue(ByteBuffer buffer, ref Rect t)
        {
            t = buffer.ReadRect();
        }

        public override void WriteValue(ByteBuffer buffer, Rect t)
        {
            buffer.WriteRect(t);
        }
    }

    public sealed class QuaternionParse : ValueParseBase<Quaternion>
    {
        public override Type Type => typeof(Quaternion);
        public override void ReadValue(ByteBuffer buffer, ref Quaternion t)
        {
            t = buffer.ReadQuaternion();
        }

        public override void WriteValue(ByteBuffer buffer, Quaternion t)
        {
            buffer.WriteQuaternion(t);
        }
    }

    public class ValueParse
    {
        public static ByteParse ByteParse;
        public static SByteParse SByteParse;
        public static BoolParse BoolParse;
        public static CharParse CharParse;
        public static ShortParse ShortParse;
        public static UShortParse UShortParse;
        public static UIntParse UIntParse;
        public static IntParse IntParse;
        public static LongParse LongParse;
        public static ULongParse ULongParse;
        public static FloatParse FloatParse;
        public static DoubleParse DoubleParse;
        public static StringParse StringParse;
        public static Vector2Parse Vector2Parse;
        public static Vector3Parse Vector3Parse;
        public static Vector4Parse Vector4Parse;
        public static Color32Parse Color32Parse;
        public static ColorParse ColorParse;
        public static DateTimeParse DateTimeParse;
        public static RectParse RectParse;
        public static QuaternionParse QuaternionParse;
        static ValueParse()
        {
            ByteParse = new ByteParse();
            SByteParse = new SByteParse();
            BoolParse = new BoolParse();
            CharParse = new CharParse();
            ShortParse = new ShortParse();
            UShortParse = new UShortParse();
            UIntParse = new UIntParse();
            IntParse = new IntParse();
            LongParse = new LongParse();
            ULongParse = new ULongParse();
            FloatParse = new FloatParse();
            DoubleParse = new DoubleParse();
            StringParse = new StringParse();
            Vector2Parse = new Vector2Parse();
            Vector3Parse = new Vector3Parse();
            Vector4Parse = new Vector4Parse();
            Color32Parse = new Color32Parse();
            ColorParse = new ColorParse();
            DateTimeParse = new DateTimeParse();
            RectParse = new RectParse();
            QuaternionParse = new QuaternionParse();
        }
       
        public static T ReadValue<T>(ByteBuffer buffer, ValueParseBase<T> parse)
        {
            T t = default(T);
            parse.ReadValue(buffer,ref t);
            return t;
        }

        public static T[] ReadArrayValue<T>(ByteBuffer buffer, ValueParseBase<T> parse)
        {
            byte length = buffer.ReadByte();
            if (length > 0)
            {
                T[] t = new T[length];
                for (int i = 0; i < length; i++)
                {
                    parse.ReadValue(buffer, ref t[i]);
                }
                return t;
            }
            return null;
        }

        public static Dictionary<TK, TV> ReadDictionary<TK, TV>(ByteBuffer buffer, ValueParseBase<TK> kparse,
            ValueParseBase<TV> vparse)
        {
            byte length = buffer.ReadByte();
            if (length > 0)
            {
                Dictionary<TK, TV> t = new Dictionary<TK, TV>();
                for (int i = 0; i < length; i++)
                {
                    TK k = ReadValue(buffer, kparse);
                    TV v = ReadValue(buffer, vparse);
                    t.Add(k,v);
                }
                return t;
            }
            return null;
        }   

        public static void WriteValue<T>(ByteBuffer buffer, T t, ValueParseBase<T> parse)
        {
            parse.WriteValue(buffer,t);
        }

        public static void WriteArrayValue<T>(ByteBuffer buffer, T[] t, ValueParseBase<T> parse)
        {
            if (t == null || t.Length == 0)
            {
                buffer.WriteByte(0);
            }
            else
            {
                buffer.WriteByte((byte)t.Length);
                foreach (T t1 in t)
                {
                    parse.WriteValue(buffer,t1);
                }
            }
        }
        
        public static void WriteDictionaryValue<TK,TV>(ByteBuffer buffer,Dictionary<TK,TV> t, ValueParseBase<TK> kparse,ValueParseBase<TV> vparse)
        {
            if (t == null || t.Count == 0)
            {
                buffer.WriteByte(0);
            }
            else
            {
                buffer.WriteByte((byte)t.Count);
                foreach (KeyValuePair<TK,TV> keyValuePair in t)
                {
                    WriteValue(buffer,keyValuePair.Key,kparse);
                    WriteValue(buffer,keyValuePair.Value,vparse);
                }
            }
        }
        
    }
}