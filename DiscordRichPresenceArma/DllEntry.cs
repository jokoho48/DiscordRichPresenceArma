#region

using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
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
            Timestamps = Timestamps.Now,
            Assets = new Assets()
            {
                LargeImageKey = "default",
                LargeImageText= "Arma 3",
            }
        };

        /// <summary>
        /// The discord client
        /// </summary>
        private static readonly DiscordRpcClient Client;

        public static Timestamps MissionStartUpTime = Timestamps.Now;
        static DllEntry()
        {
            CosturaUtility.Initialize();
            Client = new DiscordRpcClient("554274274949595146")
            {
                Logger = new ConsoleLogger()
            };
            Client.Initialize();

            //Send a presence. Do this as many times as you want
            Client.SetPresence(DefaultPresence);
        }

        ~DllEntry()
        {
            Dispose();
        }

        public static void Dispose()
        {
            Client.Dispose();
            Client.ClearPresence();
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
                    MissionStartUpTime = null;
                    Client.SetPresence(DefaultPresence);
                    break;
                case "close":
                    Client.Dispose();
                    break;
                case "init":
                    Client.Logger.Trace("Client Started");
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
            if (function == "presenceUpdate")
            {
                if (args.Length != 4) return 0;
                Client.SetPresence(new RichPresence
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
                Client.Invoke();
            } else if (function == "serverStartTimeUpdate")
            {
                if (double.TryParse(args[0], out double result))
                {
                    //MissionStartUpTime = new Timestamps(DateTime.Now.AddSeconds(result));
                    //MissionStartUpTime = Timestamps.FromTimeSpan(result);
                    MissionStartUpTime = Timestamps.Now; // TODO: Find a way to set this in the past
                }
                else
                {
                    Client.Logger.Trace($"No Valid Server Start Time Data! {args[0]}");
                }
            }
            return 0;
        }
    }
}