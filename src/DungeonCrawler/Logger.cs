using System;
using System.Globalization;
using System.IO;

namespace DungeonCrawler
{
    public class Logger
    {
        public static void Log(string message, string fileName)
        {
            var logFile = fileName + ".log";
            var path = Directory.GetCurrentDirectory() + @"\" + logFile;
            using var stream = File.AppendText(path);
            stream.WriteLine(DateTime.Now.ToString("H:mm:ss", CultureInfo.InvariantCulture) + ": " + message);
        }
    }
}