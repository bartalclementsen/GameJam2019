using System;

namespace Core.Loggers
{
    public interface ILoggerFactory
    {
        ILogger Create(UnityEngine.Object unityContext);

        ILogger Create(object context);
    }
}