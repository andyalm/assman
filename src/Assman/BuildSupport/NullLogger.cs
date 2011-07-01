using System;

namespace Assman.BuildSupport
{
    internal class NullLogger : ILogger
    {
        private static readonly ILogger _instance = new NullLogger();

        public static ILogger Instance
        {
            get { return _instance; }
        }

        public void LogMessage(string message)
        {
            //no-op
        }
    }
}