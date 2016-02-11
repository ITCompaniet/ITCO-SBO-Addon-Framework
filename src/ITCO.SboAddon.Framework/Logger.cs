using System;

namespace ITCO.SboAddon.Framework
{
    public static class Logger
    {
        public enum LogLevel
        {
            Info,
            Debug,
            Warning,
            Error            
        }

        private static void Log(string message, LogLevel logLevel, Exception exception = null)
        {

        }

        public static void Info(string message)
        {
            Log(message, LogLevel.Info);
        }

        public static void Debug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        public static void Warning(string message)
        {
            Log(message, LogLevel.Warning);
        }

        public static void Error(string message, Exception exception)
        {
            Log(message, LogLevel.Error, exception);
        }
    }
}
