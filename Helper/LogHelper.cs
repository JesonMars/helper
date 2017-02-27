using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Helper
{
    public static class LogHelper
    {
        public static ILog Logger = LogManager.GetLogger("logger");

        public static void Log(string message,Exception ex,LogType logType)
        {
            switch (logType)
            {
                case LogType.Info:
                    Logger.Info(message,ex);
                    break;
                case LogType.Error:
                    Logger.Info(message,ex);
                    break;
                case LogType.Fatal:
                    Logger.Fatal(message,ex);
                    break;
                case LogType.Warn:
                    Logger.Warn(message, ex);
                    break;
                default:
                    Logger.Info(message,ex);
                        break;
            }
        }

        public enum LogType
        {
            Info,
            Error,
            Fatal,
            Warn
        }
    }
}
