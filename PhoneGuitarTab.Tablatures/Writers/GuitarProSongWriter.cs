using System;
using System.IO;
using System.Text;
using PhoneGuitarTab.Tablatures.Models;

namespace PhoneGuitarTab.Tablatures.Writers
{
    public abstract class GuitarProSongWriter
    {
        protected BinaryWriter StreamWriter;

        protected GuitarProSongWriter(BinaryWriter sw)
        {
            StreamWriter = sw;
        }

        public abstract void WriteSong(Song song);

        #region Protected methods

        protected void SkipBytes(int count)
        {
            for (int i = 0; i < count; i++)
            {
                StreamWriter.Write((byte) 0);
            }
        }

        protected void WriteByte(byte v)
        {
            StreamWriter.Write(v);
        }

        protected void WriteByte(sbyte v)
        {
            StreamWriter.Write(v);
        }

        protected void WriteUnsignedByte(int v)
        {
            StreamWriter.Write((byte) v);
        }

        protected void WriteBytes(sbyte[] v)
        {
            foreach (var @sbyte in v)
            {
                StreamWriter.Write(@sbyte);
            }
        }

        protected void WriteBoolean(bool v)
        {
            StreamWriter.Write((byte) (v ? 1 : 0));
        }

        protected void WriteInt(int v)
        {
            byte[] bytes =
            {
                (byte) (v & 0x00FF),
                (byte) ((v >> 8) & 0x000000FF),
                (byte) ((v >> 16) & 0x000000FF),
                (byte) ((v >> 24) & 0x000000FF)
            };
            StreamWriter.Write(bytes);
        }

        protected void WriteString(byte[] bytes, int maximumLength)
        {
            int length = (maximumLength == 0 || maximumLength > bytes.Length ? bytes.Length : maximumLength);
            for (int i = 0; i < length; i++)
            {
                StreamWriter.Write(bytes[i]);
            }
        }

        protected void WriteStringInteger(String s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            WriteInt(bytes.Length);
            WriteString(bytes, 0);
        }


        protected void WriteStringByte(String s, int size)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            WriteByte((byte) (size == 0 || size > bytes.Length ? bytes.Length : size));
            WriteString(bytes, size);
            SkipBytes(size - bytes.Length);
        }


        protected void WriteStringByteSizeOfInteger(String s)
        {
            s = s ?? "";
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            WriteInt((bytes.Length + 1));
            WriteStringByte(s, bytes.Length);
        }


        protected void Close()
        {
            StreamWriter.Close();
        }

        #endregion
    }
}