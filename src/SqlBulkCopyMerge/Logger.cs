using System;

namespace SqlBulkCopyMerge
{
    public interface ILogger
    {
        public void Verbose(string message, params object[] args);
        public void Debug(string message, params object[] args);
        public void Warning(string message, params object[] args);
        public void Warning(Exception ex, string message, params object[] args);
        public void Error(string message, params object[] args);
        public void Error(Exception ex, string message, params object[] args);
    }

    public static class Log
    {
        public static ILogger Logger { get; set; } = new EmptyLogger();

        public static void Verbose(string message, params object[] args)
        {
            Logger?.Verbose(message, args);
        }

        public static void Debug(string message, params object[] args)
        {
            Logger?.Debug(message, args);
        }

        public static void Warning(string message, params object[] args)
        {
            Logger?.Warning(message, args);
        }

        public static void Error(string message, params object[] args)
        {
            Logger?.Error(message, args);
        }
    }

    internal class EmptyLogger : ILogger
    {
        public void Verbose(string message, params object[] args)
        {
        }

        public void Debug(string message, params object[] args)
        {
        }

        public void Warning(string message, params object[] args)
        {
        }

        public void Warning(Exception ex, string message, params object[] args)
        {
        }

        public void Error(string message, params object[] args)
        {
        }

        public void Error(Exception ex, string message, params object[] args)
        {
        }
    }
}
