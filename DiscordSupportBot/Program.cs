//#define SER
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordSupportBot
{
    class Program
    {
#if (SER)

        public static void Main(string[] args)
        {
            Config.singletone.Serialize();
        }
#else
        public static Program singletone;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public DiscordSocketClient client { get; private set; }
        public CommandService commands { get; private set; }

        public Program()
        {
            singletone = this;

            DiscordSocketConfig socketConfig = new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50
            };
            client = new DiscordSocketClient(socketConfig);

            CommandServiceConfig commandsConfig = new CommandServiceConfig()
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false
            };
            commands = new CommandService(commandsConfig);

            Logger.singletone = new Logger(client, commands);
            CommandHandler.singletone = new CommandHandler(client, commands);

            client.Ready += OnClientReady;
            client.Disconnected += Client_Disconnected;
            
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Config.singletone.TrySerialize();
            RuntimeData.ClearRooms();
        }

        private Task Client_Disconnected(Exception arg)
        {
            Config.singletone.TrySerialize();
            return Task.CompletedTask;
        }

        public async Task MainAsync()
        {
            try
            {
                await CommandHandler.singletone.InstallCommandsAsync();

                await client.LoginAsync(TokenType.Bot, Config.singletone.botToken);
                await client.StartAsync();

                client.ReactionAdded += Client_ReactionAdded;
                client.UserVoiceStateUpdated += Client_UserVoiceStateUpdated;

                await Task.Delay(Timeout.Infinite);
                Config.singletone.TrySerialize();
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }

        private Task Client_UserVoiceStateUpdated(SocketUser user, SocketVoiceState voiceState1, SocketVoiceState voiceState2)
        {
            if (voiceState2.VoiceChannel != null && voiceState2.VoiceChannel.Id == Config.singletone.TempRoomCreationChannelId)
            {
                var channel = voiceState2.VoiceChannel.Guild.CreateVoiceChannelAsync(user.Username).Result;
                OverwritePermissions creatorPermissions = new OverwritePermissions(
                    PermValue.Allow, PermValue.Allow, PermValue.Inherit, 
                    PermValue.Allow, PermValue.Inherit, PermValue.Inherit, 
                    PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, 
                    PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, 
                    PermValue.Allow, PermValue.Allow, PermValue.Deny, PermValue.Deny, 
                    PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Deny);
                channel.AddPermissionOverwriteAsync(voiceState2.VoiceChannel.Guild.GetUser(user.Id), creatorPermissions);
                channel.ModifyAsync(x => x.CategoryId = Config.singletone.TempRoomCreationChannelCategoryId);
                var user1 = voiceState2.VoiceChannel.Guild.GetUser(user.Id);
                user1.ModifyAsync(x => x.Channel = channel);

                RuntimeData.createdTempChannels.Add((voiceState2.VoiceChannel.Guild.Id, channel.Id));
            }
            else if (voiceState1.VoiceChannel != null)
            {
                var leftChannel = voiceState1.VoiceChannel;
                if (RuntimeData.IsChannelTempCreated(leftChannel.Guild.Id, leftChannel.Id))
                {
                    if (leftChannel.Users.Count <= 0)
                    {
                        leftChannel.DeleteAsync();
                        RuntimeData.createdTempChannels.Remove((leftChannel.Guild.Id, leftChannel.Id));
                    }
                }
            }
            return Task.CompletedTask;
        }

        private Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var relation = Config.singletone.rulesAcceptRelation;
            if (channel.Id == relation.channelId)
            {
                var currentGuild = client.GetGuild(relation.guildID);
                currentGuild.GetUser(reaction.UserId).RemoveRoleAsync(currentGuild.GetRole(relation.remRoleId));
                currentGuild.GetUser(reaction.UserId).AddRoleAsync(currentGuild.GetRole(relation.addRoleId));
            }
            return Task.CompletedTask;
        }

        private Task OnClientReady()
        {
            Logger.Log("Bot connected");
            return Task.CompletedTask;
        }
#endif

    }
}
