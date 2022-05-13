using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordSupportBot.Modules
{
    public class BotSetup : ModuleBase<SocketCommandContext>
    {
        [Command("strcc")]
        [Summary("Set voice channel for creating temporary voice channels")]
        public async Task SetTempRoomCreationChannel(ulong channelId, ulong categoryId)
        {
            Config.singletone.TempRoomCreationChannelId = channelId;
            Config.singletone.TempRoomCreationChannelCategoryId = categoryId;
            Config.singletone.TrySerialize();
            await Context.Channel.SendMessageAsync("Temp room creation channel is now ready");
        }

        [Command("surar")]
        [Summary("Gives and removes roles from user who added emote in text channel")]
        public async Task SetUserRoleAfterReact(IChannel channel, string emoteName, IRole addRole, IRole remRole)
        {
            if (channel == null || addRole == null || string.IsNullOrEmpty(emoteName))
            {
                await Context.Channel.SendMessageAsync("Missing arguments! (channel, addRole, emotename, remRole");
                return;
            }

            Config.singletone.rulesAcceptRelation = new Config.MessageReactRoleRelation(addRole.Id, emoteName, remRole != null ? remRole.Id : 18446744073709551614, channel.Id, Context.Guild.Id);
            Config.singletone.TrySerialize();
            await Context.Channel.SendMessageAsync("React role is now set");
        }
    }
}
