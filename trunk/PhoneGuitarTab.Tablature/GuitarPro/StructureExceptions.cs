using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PhoneGuitarTab.Tablature.GuitarPro
{
   // [Serializable]
    public class FileStructureException : Exception
    {
        public FileStructureException() : base() { }
        public FileStructureException(string s) : base(s) { }
        public FileStructureException(string s, Exception e) : base(s, e) { }

    //    protected FileStructureException(SerializationInfo info, StreamingContext cxt) : base(info, cxt) { }
    }

    //[Serializable]
    public class HeaderFileStructureException : FileStructureException
    {
        public HeaderFileStructureException() : base() { }
        public HeaderFileStructureException(string s) : base(s) { }
        public HeaderFileStructureException(string s, Exception e) : base(s, e) { }

     //   protected HeaderFileStructureException(SerializationInfo info, StreamingContext cxt) : base(info, cxt) { }
    }

   // [Serializable]
    public class BodyFileStructureException : FileStructureException
    {
        public BodyFileStructureException() : base() { }
        public BodyFileStructureException(string s) : base(s) { }
        public BodyFileStructureException(string s, Exception e) : base(s, e) { }

     //   protected BodyFileStructureException(SerializationInfo info, StreamingContext cxt) : base(info, cxt) { }
    }

}
