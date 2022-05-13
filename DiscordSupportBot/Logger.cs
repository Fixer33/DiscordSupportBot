using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordSupportBot
{
    class Logger
    {
        public static Logger singletone;

        public Logger(DiscordSocketClient client, CommandService commands)
        {
            client.Log += Log;
            commands.Log += Log;
        }

        public Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message} {msg.Exception}");
            Console.ResetColor();

            return Task.CompletedTask;
        }

        public static void Log(string text)
        {
            Console.WriteLine($"{DateTime.Now,-19} [Info] Bot: {text}");
        }
    }
}
