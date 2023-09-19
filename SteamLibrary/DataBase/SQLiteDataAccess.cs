using SteamLibrary.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Configuration.Assemblies;
using System.Configuration;

namespace SteamLibrary.DataBase
{
    public class ConfigManager
    {
        public static void ProtectConnectionStrings() {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSection strings = config.GetSection("connectionStrings");
            if (!strings.SectionInformation.IsProtected)
            {
                strings.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                config.Save();
            }
        }
    }
}
