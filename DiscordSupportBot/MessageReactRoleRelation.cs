using System;
namespace DiscordSupportBot
{
    public partial class Config
    {
        [Serializable]
        public class MessageReactRoleRelation
        {
            public string emoteName = " ";
            public ulong addRoleId = 9999999;
            public ulong remRoleId = 9999999;
            public ulong channelId = 9999999;
            public ulong guildID = 9999999;

            public MessageReactRoleRelation(ulong addRoleId, string emoteName, ulong remRoleId, ulong channelId, ulong guildId)
            {
                this.emoteName = emoteName;
                this.addRoleId = addRoleId;
                this.remRoleId = remRoleId;
                this.channelId = channelId;
                this.guildID = guildId;
            }
        }

    }


}
