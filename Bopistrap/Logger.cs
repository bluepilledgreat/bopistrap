using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public static void WriteLine(object? message)
        {
            Debug.WriteLine(message);
            WriteToFile(message);
        }

        static Logger()
        {
            InitialiseWriter();
        }
    }
}
