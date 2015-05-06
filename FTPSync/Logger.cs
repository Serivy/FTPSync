using System;
using System.Globalization;
using System.IO;

namespace FTPSync
{
    public class Logger
    {
        private const string LogFileName = "log.txt";
        private readonly string logFile;

        public Logger(string logDir)
        {
            logFile = Path.Combine(logDir, LogFileName);
        }

        public void Log(string text)
        {
            File.AppendAllText(this.logFile, string.Format("{0}: {1}{2}", DateTime.Now.ToString(CultureInfo.CurrentCulture), text, Environment.NewLine));
        }
    }
}
