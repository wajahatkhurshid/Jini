using System;
using System.Runtime.CompilerServices;

namespace Gyldendal.Jini.Services.Utils
{
    public interface ILogger
    {
        void LogError(string message, Exception ex, bool isCritical = false, [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);
    }
}