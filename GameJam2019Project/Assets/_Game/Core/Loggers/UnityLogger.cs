using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Core.Loggers
{
    public class UnityLogger : ILogger
    {
        private readonly Logger _logger;

        /// <summary>
        /// Object to which the message applies.
        /// </summary>
        private UnityEngine.Object _unityContext;

        /// <summary>
        /// Used to identify the source of a log message. It usually identifies the class where the log call occurs.
        /// </summary>
        private readonly string _tag;

        private readonly StringBuilder _stringBuilder;

        public UnityLogger(UnityEngine.Object unityContext) : this()
        {
            _unityContext = unityContext;
            _tag = unityContext?.name ?? unityContext?.GetType().Name;
        }

        public UnityLogger(object context) : this()
        {
            _tag = context?.GetType().Name;
        }

        private UnityLogger()
        {
            _stringBuilder = new StringBuilder();
            _logger = new Logger(new LogHandler());
        }

        public void Error(object message)
        {
            DoLog(message, LogType.Error);
        }

        public void Warning(object message)
        {
            DoLog(message, LogType.Warning);
            LogMethodCall(new object[] { "", 1, 1 });
        }

        public void LogMethodCall(object[] args = null, [CallerMemberName]string method = "")
        {
            _stringBuilder.Clear();

            _stringBuilder.Append(method);
            _stringBuilder.Append("(");

            if(args != null)
            {
                _stringBuilder.Append(string.Join(", ", args));
            }

            _stringBuilder.Append(")");

            Log(_stringBuilder.ToString());
        }

        public void Log(object message)
        {
            DoLog(message, LogType.Log);
        }

        public void Exception(Exception exception)
        {
            if (_unityContext == null)
                _logger.LogException(exception);
            else
                _logger.LogException(exception, _unityContext);
        }

        private void DoLog(object message, LogType logType)
        {
            if(string.IsNullOrWhiteSpace(_tag) && _unityContext == null)
                _logger.Log(logType, message); 
            else
                _logger.Log(logType, _tag, message, _unityContext);
        }

        private class LogHandler : ILogHandler
        {
            public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
            {
                Debug.unityLogger.logHandler.LogFormat(logType, context, format, args);
            }

            public void LogException(Exception exception, UnityEngine.Object context)
            {
                Debug.unityLogger.LogException(exception, context);
            }
        }
    }
}