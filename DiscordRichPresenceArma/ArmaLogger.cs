using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC.Logging;
namespace DiscordRichPresenceArma
{
    public class ArmaLogger: DiscordRPC.Logging.ILogger
    {
        private const char SplitChar = '\x01';
        public LogLevel Level { get; set; }
        private List<string> Logs = new List<string>();


        public string ReadLogs()
        {
            StringBuilder result = new StringBuilder();
            foreach (string log in Logs)
            {
                result.Append($"{log}{SplitChar}");
            }
            Logs.Clear();
            result.Remove(result.Length - 1, 1);
            return result.ToString();
        }
        public ArmaLogger()
        {
            this.Level = LogLevel.Info;
        }

        public void Trace(string message, params object[] args)
        {
            if (Level > LogLevel.Trace) return;
            Logs.Add($"TRCE: {(args.Length > 0 ? string.Format(message, args) : message)}");
        }

        public void Info(string message, params object[] args)
        {
            if (Level > LogLevel.Info) return;
            Logs.Add($"INFO: {(args.Length > 0 ? string.Format(message, args) : message)}");

        }

        public void Warning(string message, params object[] args)
        {
            if (Level > LogLevel.Warning) return;
            Logs.Add($"WARN: {(args.Length > 0 ? string.Format(message, args) : message)}");

        }

        public void Error(string message, params object[] args)
        {
            if (Level > LogLevel.Error) return;
            Logs.Add($"Err: {(args.Length > 0 ? string.Format(message, args) : message)}");

        }

    }
}
