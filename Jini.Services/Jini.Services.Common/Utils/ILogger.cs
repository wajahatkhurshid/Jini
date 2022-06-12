using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Gyldendal.Jini.Services.Utils
{
    public interface ILogger
    {
        void LogInfo(string message,
          bool forced = false,
          [CallerMemberName] string methodName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0,
          string transactionId = null,
          bool isGdprSafe = false);

        void Warning(string message,
            bool forced = false,
            [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            string transactionId = null,
            bool isGdprSafe = false);

        void Debug(string message,
            [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            string transactionId = null,
            bool isGdprSafe = false);

        void LogError(string message,
            Exception ex,
            bool isCritical = false,
            [CallerMemberName] string methodName = "",
            string transactionId = null,
            bool isGdprSafe = false);

        void Critical(string message,
            Exception ex,
            [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            string transactionId = null,
            bool isGdprSafe = false);

        void Fatal(string message,
            Exception ex,
            [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            string transactionId = null,
            bool isGdprSafe = false);

        string GetPropertyNameAndValue(params Expression<Func<object>>[] propertyLambda);

        void SetCurrentThreadContext(string context);

        void ShutdownLogger();
    }
}