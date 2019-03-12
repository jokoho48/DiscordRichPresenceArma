using System;
using System.Linq;
using System.Text;
using DiscordRichPresenceArma;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DllEntry.RvExtension(new StringBuilder(9999),9999, "version");
            
            string input = "Altis";
            while (input != "exit")
            {
                DllEntry.RvExtensionArgs(new StringBuilder(9999),9999, "serverStartTimeUpdate", new []{"600"}, 1);
                DllEntry.RvExtensionArgs(new StringBuilder(9999),9999, "presenceUpdate", new []{"Test Mission", input, "Test Server Name", input.ToLowerInvariant()}, 4);
                input = Console.ReadLine();
            }

            DllEntry.Dispose();
        }
    }
}
