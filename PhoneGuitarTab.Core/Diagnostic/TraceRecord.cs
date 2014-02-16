
namespace PhoneGuitarTab.Core.Diagnostic
{
    using System;

    public sealed class TraceRecord
    {
        public TraceCategory Category { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Page { get; set; }
        public Exception Exception { get; set; }
        public Type SourceType { get; set; }

        public TraceRecord()
        {
            Date = DateTime.MinValue;
        }
    }
}
