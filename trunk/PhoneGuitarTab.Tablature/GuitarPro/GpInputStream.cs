using System;

using System.IO;
using System.Text;
using System.Windows.Media;
using PhoneGuitarTab.Tablature;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
    public class GpInputStream
    {
        protected Stream stream;
        protected Version Version { get; set; }

        public GpInputStream(Stream fileStream)
        {
            stream = fileStream;
        }

       /* public static GetGpFile GetGpFile(Stream fileStream)
        {
            Version version = GetVersionFromString();
            switch(version.Major)
            {
                case 5:
                    return new Gp5InputStream(fileStream);
                case 4:
                    break;
            }


        }*/

       internal Version GetVersionFromString()
       {
           string version = ReadString(31);

           if (version.Contains("FICHIER GUITAR PRO v3.00"))
           {
               Version = new Version() { Major = "3", Minor = "00" };
               return Version;
           }
           if (version.Contains("FICHIER GUITAR PRO v4.00"))
           {
               Version = new Version() { Major = "4", Minor = "00" };
               return Version;
           }
           if (version.Contains("FICHIER GUITAR PRO v4.06"))
           {
               Version = new Version() {Major = "4", Minor = "06"};
               return Version;
           }
           if (version.Contains("FICHIER GUITAR PRO v5.00"))
           {
               Version = new Version() {Major = "5", Minor = "00"};
               return Version;
           }
           if (version.Contains("FICHIER GUITAR PRO v5.10"))
           {
               Version = new Version() {Major = "5", Minor = "10"};
               return Version;
           }

           throw new HeaderFileStructureException(SR.UnsupportedFileVersion);
        }

       protected byte[] ReadBuffer( int size)
       {
            return ReadBuffer(0, size);
        }

       protected byte[] ReadBuffer(int position, int size)
       {
            byte[] buffer = new byte[size];
            stream.Read(buffer, position, size);
            return buffer;
        }

        /// <summary>
        /// Reads size bytes and interprets it as string
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="size"></param>
        /// <returns></returns>
       protected string ReadString(int size)
       {
            return Encoding.UTF8.GetString(ReadBuffer(size), 0 ,size);
        }

        /// <summary>
        /// Piece information is presented in the form of a block of data containing:
	    /// an integer representing the size of the stored information + 1;
	    /// the string of characters representing the data
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected string ReadHeaderEntry()
        {
            string headerEntry = ReadStringInteger();
            return String.IsNullOrEmpty(headerEntry) ? "" : headerEntry.Substring(1);
        }

       protected string ReadStringInteger()
        {
            int titleSize = BitConverter.ToInt32(ReadBuffer( sizeof(Int32)), 0);
            return titleSize > 0 ? Encoding.UTF8.GetString(ReadBuffer(titleSize),0,titleSize) : "";
        }

        protected int ReadInt(){
            return BitConverter.ToInt32(ReadBuffer(sizeof(Int32)), 0);
        }

        protected byte ReadByte(){
            return (byte)stream.ReadByte();
        }

        protected string[] ReadStringArrayWithLength(){
            var size = ReadInt();
            string[] array = new string[size];
            for (int i = 0; i < size; i++)
                array[i] = ReadHeaderEntry();

            return array;
        }

        protected void Skip(int bytes)
        {
            stream.Seek(bytes, SeekOrigin.Current);
        }

        protected Key ReadKey(){
            return (Key) (SByte) ReadByte();
        }
  

        protected Duration ReadDuration()
        {
            return (Duration)(SByte)ReadByte();
        }

        protected T ReadEnum<T>(){
            return (T)Enum.ToObject(typeof(T), (SByte)ReadByte());
        }

        protected Color ReadColor()
        {
            byte red = (byte)stream.ReadByte();
            byte green = (byte)stream.ReadByte();
            byte blue = (byte)stream.ReadByte();
            byte alpha = (byte)stream.ReadByte();
            return Color.FromArgb(alpha, red, green, blue);
        }


    }
}