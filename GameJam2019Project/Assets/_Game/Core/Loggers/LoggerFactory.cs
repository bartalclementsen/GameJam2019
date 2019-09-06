using System;

namespace Core.Loggers
{
    public class LoggerFactory : ILoggerFactory
    {
        public ILogger Create(object context)
        {
            return new UnityLogger(context);
        }

        public ILogger Create(UnityEngine.Object unityContext)
        {
            return new UnityLogger(unityContext);
        }
    }
}