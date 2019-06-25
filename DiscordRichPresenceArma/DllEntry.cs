#region

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using DiscordRPC;
using DiscordRPC.Logging;
#endregion

namespace DiscordRichPresenceArma
{
    public class DllEntry
    {
        /// <summary>
        /// The current presence to send to discord.
        /// </summary>
        private static readonly RichPresence DefaultPresence = new RichPresence()
        {
            Details = "This is a Test",
            State = "In Menu",
            Timestamps = new Timestamps(DateTime.UtcNow),
            Assets = new Assets()
            {
                LargeImageKey = "default",
                LargeImageText= "Arma 3",
            }
        };

        private const string DefaultAppId = "554274274949595146";
        private static string _appid = DefaultAppId;

        /// <summary>
        /// The discord client
        /// </summary>
        private static DiscordRpcClient _client;

        private static Timestamps MissionStartUpTime = Timestamps.Now;
        static DllEntry()
        {
            CosturaUtility.Initialize();
            InitClient();
        }

        ~DllEntry()
        {
            Dispose();
        }

        private static void InitClient()
        {
            _client = new DiscordRpcClient(_appid)
            {
#if DEBUG
                //Logger = new ArmaLogger {Level = LogLevel.Trace}
                Logger = new ConsoleLogger { Level = LogLevel.Trace, Coloured = true }
#endif
            };
            _client.Initialize();

            //Send a presence. Do this as many times as you want
            _client.SetPresence(DefaultPresence);
            _client.Invoke();
        }
        public static void Dispose()
        {
            _client.Dispose();
            _client.ClearPresence();
        }
        /// <summary>
        ///     Gets called when arma starts up and loads all extension.
        ///     It's perfect to load in static objects in a seperate thread so that the extension doesn't needs any seperate
        ///     initalization
        /// </summary>
        /// <param name="output">The string builder object that contains the result of the function</param>
        /// <param name="outputSize">The maximum size of bytes that can be returned</param>
#if WIN64
        [DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionVersion@8", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RvExtensionVersion(StringBuilder output, int outputSize)
        {
            output.Append(FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(DllEntry)).Location).FileVersion);
        }

        /// <summary>
        ///     The entry point for the default callExtension command.
        /// </summary>
        /// <param name="output">The string builder object that contains the result of the function</param>
        /// <param name="outputSize">The maximum size of bytes that can be returned</param>
        /// <param name="function">The string argument that is used along with callExtension</param>
#if WIN64
        [DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RvExtension(StringBuilder output, int outputSize,
            [MarshalAs(UnmanagedType.LPStr)] string function)
        {
            function = function.ToLowerInvariant();


            switch (function)
            {
                case "version":
                    output.Append(FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(DllEntry)).Location).FileVersion);
                    break;
                case "end":
                    _appid = DefaultAppId;
                    InitClient();
                    MissionStartUpTime = null;
                    _client.SetPresence(DefaultPresence);
                    break;
                case "close":
                    Dispose();
                    break;
                case "init":
                    _client.Logger.Trace("Client Started");
                    break;
                case "readlogs":
                    output.Append(((ArmaLogger) _client.Logger).ReadLogs());
                    break;
            }
        }

        /// <summary>
        ///     The entry point for the callExtensionArgs command.
        /// </summary>
        /// <param name="output">The string builder object that contains the result of the function</param>
        /// <param name="outputSize">The maximum size of bytes that can be returned</param>
        /// <param name="function">The string argument that is used along with callExtension</param>
        /// <param name="args">The args passed to callExtension as a string array</param>
        /// <param name="argCount">The size of the string array args</param>
        /// <returns>The result code</returns>
#if WIN64
        [DllExport("RVExtensionArgs", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionArgs@20", CallingConvention = CallingConvention.Winapi)]
#endif
        public static int RvExtensionArgs(StringBuilder output, int outputSize,
            [MarshalAs(UnmanagedType.LPStr)] string function,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 4)]
            string[] args, int argCount)
        {
            switch (function)
            {
                case "presenceUpdate" when args.Length != 4:
                    _client.Logger.Trace("No Valid PresenceUpdate Data count");
                    break;
                case "presenceUpdate":
                    _client.SetPresence(new RichPresence
                    {
                        Details = $"{'"'}{args[0]}{'"'} on {args[1]}",
                        State = $"{(args[2] == "" ? "In Singleplayer Mission" : $"On {'"'}{ args[2]}{'"'} Server")}",
                        Timestamps = MissionStartUpTime,
                        Assets = new Assets
                        {
                            LargeImageKey = args[3],
                            LargeImageText= args[1],
                            SmallImageKey = "default",
                            SmallImageText = "Arma 3"
                        }
                    });
                    _client.Invoke();
                    break;
                case "serverStartTimeUpdate" when double.TryParse(args[0], out double result):
                    TimeSpan ts = TimeSpan.FromSeconds(result);
                    MissionStartUpTime = new Timestamps(DateTime.UtcNow - ts, null);
                    break;
                case "serverStartTimeUpdate":
                    _client.Logger.Trace($"No Valid Server Start Time Data! {args[0]}");
                    break;
                case "customAppId":
                    _appid = args[0];
                    InitClient();
                    break;

            }

            return 0;
        }
    }
}
