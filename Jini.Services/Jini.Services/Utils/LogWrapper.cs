using System;
using LoggingManager;

namespace Gyldendal.Jini.Services.Utils
{
    public class LogWrapper : ILogger
    {
        public void LogError(string message, Exception ex, bool isCritical = false, string methodName = "", string sourceFilePath = "",
            int sourceLineNumber = 0)
        {
            Logger.Instance.LogError(message, ex, isCritical, methodName, sourceFilePath, sourceLineNumber);
        }
    }
}