using System;
using UnityEngine;

namespace Core.Loggers
{
    public interface ILogger
    {
        void LogMethodCall(object[] args, string method);
        void Error(object message);
        void Exception(Exception exception);
        void Log(object message);
        void Warning(object message);
    }
}