using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordSupportBot
{
    class RuntimeData
    {
        public static List<(ulong guildId, ulong channelId)> createdTempChannels = new List<(ulong guildId, ulong channelId)>();

        public static bool IsChannelTempCreated(ulong guildId, ulong channelId)
        {
            foreach (var item in createdTempChannels)
            {
                if (item.guildId == guildId && item.channelId == channelId)
                    return true;
            }
            return false;
        }
        public static void ClearRooms()
        {
            foreach (var item in createdTempChannels)
            {
                Program.singletone.client.GetGuild(item.guildId).GetChannel(item.channelId).DeleteAsync();
            }
        }
    }
}
