using UnityEngine;
using System.Collections;
using System;

namespace I2.TextAnimation
{
	public class ArrayBuffer<T>
	{
		public int Size;
		public T[] Buffer = new T[0];


		public void ReserveExtra(int newElements)
		{
            int newSize = Size + newElements;
            if (Buffer.Length < newSize)
			    Array.Resize (ref Buffer, newSize);
		}

		public void ReserveTotal(int Capacity)
		{
			if (Buffer.Length< Capacity)
				Array.Resize (ref Buffer, Capacity);
		}

        public void Clear(int Capacity=-1)
        {
            if (Capacity > 0 && Buffer.Length < Capacity)
                Buffer = new T[Capacity];
            Size = 0;
        }

        public void Reset(int NewSize=-1)
		{
            if (NewSize > 0 && Buffer.Length < NewSize)
                Buffer = new T[NewSize];
            Size = NewSize>0 ? NewSize : 0;
        }

        public void CopyFrom( ArrayBuffer<T> from, int fromIndex=-1, int toIndex=-1, int count=-1, bool trim=true)
        {
            if (fromIndex < 0) fromIndex = 0;
            if (count < 0) count = from.Size - fromIndex;
            if (toIndex < 0) toIndex = Size;
            var newSize = toIndex + count;

            if (Buffer.Length < newSize)
            {
                if (Size > 0)
                    Array.Resize(ref Buffer, newSize);
                else
                    Buffer = new T[newSize];
                Size = newSize;
            }
            else
            if (trim)
                Size = newSize;

            Array.Copy(from.Buffer, fromIndex, Buffer, toIndex, count);
        }
    }
}