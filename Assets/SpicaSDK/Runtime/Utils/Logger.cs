using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpicaSDK.Runtime.Utils
{
    public static class SpicaLogger
    {
        public class Logger : UnityEngine.Logger
        {
            private class LogHandler : ILogHandler
            {
                public void LogFormat(LogType logType, Object context, string format, params object[] args)
                {
                    Debug.unityLogger.logHandler.LogFormat(logType, context, format, args);
                }

                public void LogException(Exception exception, Object context)
                {
                    Debug.unityLogger.LogException(exception, context);
                }
            }

            private UnityEngine.Logger logger;

            public Logger() : base(new LogHandler())
            {
            }
        }

        private static Lazy<Logger> instance = new Lazy<Logger>(() =>
        {
            var logger = new Logger();
            logger.logEnabled = false;

#if UNITY_EDITOR || DEVELOPMENT_BUILD || ENABLE_SPICA_LOGS
            logger.logEnabled = true;
#endif

            return logger;
        });

        public static Logger Instance => instance.Value;

        public static bool LogsEnabled => Instance.logEnabled;
    }
}