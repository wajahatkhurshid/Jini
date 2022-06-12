using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Gyldendal.Jini.Services.Common.Utils
{
    public class LogWrapper : Jini.Services.Utils.ILogger
    {
        private readonly LoggingManager.ILogger _logger;

        public LogWrapper(LoggingManager.ILogger logger)
        {
            _logger = logger;
        }
        public void LogInfo(string message,
               bool forced = false,
               [CallerMemberName] string methodName = "",
               [CallerFilePath] string sourceFilePath = "",
               [CallerLineNumber] int sourceLineNumber = 0,
               string transactionId = null,
               bool isGdprSafe = false)
        {
            _logger.Info(message, forced, methodName, sourceFilePath, sourceLineNumber, transactionId, isGdprSafe);
        }

        public void Warning(string message,
            bool forced = false,
            [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            string transactionId = null,
            bool isGdprSafe = false)
        {
            _logger.Warning(message, forced, methodName, sourceFilePath, sourceLineNumber, transactionId, isGdprSafe);
        }

        public void Debug(string message,
            [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            string transactionId = null,
            bool isGdprSafe = false)
        {
            _logger.Debug(message, methodName, sourceFilePath, sourceLineNumber, transactionId, isGdprSafe);
        }

        public void LogError(string message,
            Exception ex,
            bool isCritical = false,
            [CallerMemberName] string methodName = "",
            string transactionId = null,
            bool isGdprSafe = false)
        {
            _logger.Error(message, ex, isCritical, methodName, transactionId: transactionId, isGdprSafe: isGdprSafe);
        }

        public void Critical(string message,
            Exception ex,
            [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            string transactionId = null,
            bool isGdprSafe = false)
        {
            _logger.Critical(message, ex, methodName, sourceFilePath, sourceLineNumber, transactionId, isGdprSafe);
        }

        public void Fatal(string message,
            Exception ex,
            [CallerMemberName] string methodName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            string transactionId = null,
            bool isGdprSafe = false)
        {
            _logger.Fatal(message, ex, methodName, sourceFilePath, sourceLineNumber, transactionId, isGdprSafe);
        }

        public string GetPropertyNameAndValue(params Expression<Func<object>>[] propertyLambda)
        {
            return _logger.GetPropertyNameAndValue(propertyLambda);
        }

        public void SetCurrentThreadContext(string context)
        {
            _logger.SetCurrentThreadContext(context);
        }

        public void ShutdownLogger()
        {
            _logger.ShutdownLogger();
        }
    }
}