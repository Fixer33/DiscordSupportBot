using System;
using System.IO;
using System.Xml.Serialization;
namespace DiscordSupportBot
{
    [Serializable]
    public partial class Config
    {
        public static Config singletone;

        #region Settings

        public string botToken = "REPLACE ME";

        public ulong TempRoomCreationChannelId = 707273077515485324;
        public ulong TempRoomCreationChannelCategoryId = 385497103037890561;

        public MessageReactRoleRelation rulesAcceptRelation = new MessageReactRoleRelation(286177609094529024, "☑️", 286177604568875019, 729764915073515620, 267937787322302465);

        #endregion

        [NonSerialized] private const string configFileName = "config.xml";

        #region Serialization

        public bool TrySerialize()
        {
            try
            {
                if (File.Exists(configFileName))
                    File.Delete(configFileName);

                using (FileStream fs = new FileStream(configFileName, FileMode.CreateNew))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Config));
                    xs.Serialize(fs, this);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Log("Serialization error! Details: " + e.Message);
                return false;
            }
        }

        public static bool TryDeSerialize()
        {
            try
            {
                if (File.Exists(configFileName) == false)
                    return false;

                using (FileStream fs = File.Open(configFileName, FileMode.Open))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Config));
                    singletone = (Config)xs.Deserialize(fs);
                    return true;
                }
            }
            catch
            {
                Logger.Log("Deserialization error!");
                return false;
            }
        } 

#endregion

        static Config()
        {
            if (TryDeSerialize() == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No config found!");
                Console.ResetColor();
                singletone = new Config();
            }
        }

    }


}
