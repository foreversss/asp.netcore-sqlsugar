using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Agile.BaseLib.Helpers
{
    /// <summary>
    /// 日志类
    /// </summary>
    public class Logs
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public static void WriteError(Exception exception, string msg = "")
        {
            logger.Error(exception, msg);
        }

        public static void WriteInfo(string msg = "")
        {
            logger.Info(msg);
        }
    }
}
