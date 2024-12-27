using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bopistrap
{
    internal static class Logger
    {
        private static StreamWriter _writer = null!;

        private static void InitialiseWriter()
        {
            Directory.CreateDirectory(Paths.Logs);

            string timestamp = DateTime.UtcNow.ToString("O").Replace("-", "").Replace(":", "").Replace(".", "");
            string fileName = $"Bopistrap_{timestamp}.log";
            string path = Path.Combine(Paths.Logs, fileName);

            _writer = new StreamWriter(path) { AutoFlush = true };
        }

        private static void WriteToFile(object? message)
        {
            _writer.WriteLine(message?.ToString());
        }

        private static string ConstructMessage(object? message, string? filePath, int lineCount)
        {
            return $"[{DateTime.UtcNow.ToString("O")}] [{Path.GetFileName(filePath)}:{lineCount}] {message}";
        }

        public static void WriteLine(object? message, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            string printMessage = ConstructMessage(message, filePath, lineNumber);

            Debug.WriteLine(printMessage);
            WriteToFile(printMessage);
        }

        static Logger()
        {
            InitialiseWriter();
        }
    }
}
