namespace PhoneGuitarTab.Core.Diagnostic
{
    using System;

    /// <summary>
    /// Represents a tracer for tracing subsystem
    /// </summary>
    public interface ITrace : IDisposable
    {
        /// <summary>
        /// Level of tracing
        /// </summary>
        int Level { get; set; }

        /// <summary>
        /// Writes message to trace using default tracer category
        /// </summary>
        void Info(string message);

        /// <summary>
        /// Writes message to trace using category provided
        /// </summary>
        void Info(TraceCategory category, string message);

        /// <summary>
        /// Writes record to trace
        /// </summary>
        void Info(TraceRecord record);

        /// <summary>
        /// Writes message to trace using default tracer category
        /// </summary>
        void Warn(string message);

        /// <summary>
        /// Writes message to trace using category provided
        /// </summary>
        void Warn(TraceCategory category, string message);

        /// <summary>
        /// Writes record to trace
        /// </summary>
        void Warn(TraceRecord record);

        /// <summary>
        /// Writes message to trace using default tracer category
        /// </summary>
        void Error(string message, Exception exception);

        /// <summary>
        /// Writes message to trace
        /// </summary>
        void Error(TraceCategory category, string message, Exception exception);

        /// <summary>
        /// Writes record to trace
        /// </summary>
        void Error(TraceRecord record);

        /// <summary>
        /// Writes message to trace using default tracer category
        /// </summary>
        void Fatal(string message, Exception exception);

        /// <summary>
        /// Writes message to trace
        /// </summary>
        void Fatal(TraceCategory category, string message, Exception exception);

        /// <summary>
        /// Writes record to trace
        /// </summary>
        void Fatal(TraceRecord record);

        /// <summary>
        /// Returns the storage of messages
        /// </summary>
        object GetUnderlyingStorage();

        /// <summary>
        /// Shows whether trace is initialized
        /// </summary>
        bool IsInitialized { get; }

    }
}
