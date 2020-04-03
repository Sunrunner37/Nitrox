﻿using System.ComponentModel;
using NitroxModel.DataStructures.GameLogic;
using NitroxModel.Helper;

namespace NitroxModel.Server
{
    public class ServerConfig
    {
        private readonly ServerConfigItem<bool> disableConsoleSetting;
        private readonly ServerConfigItem<string> gameModeSetting;
        private readonly ServerConfigItem<int> portSetting, saveIntervalSetting, maxConnectionsSetting;
        private readonly ServerConfigItem<string> saveNameSetting, serverPasswordSetting, adminPasswordSetting;
        private readonly ServerConfigItem<float> oxygenSetting, maxOxygenSetting, healthSetting, foodSetting, waterSetting, infectionSetting;

        public ServerConfig(): this(
               port: 1100,
               saveinterval: 60000,
               maxconnection: 100,
               disableconsole: false,
               savename: "world",
               serverpassword: string.Empty,
               adminpassword: StringHelper.GenerateRandomString(12),
               gamemodeSetting: ServerGameMode.SURVIVAL
        ) { }

        public ServerConfig(int port, int saveinterval, int maxconnection, bool disableconsole, string savename, string serverpassword, string adminpassword, ServerGameMode gamemodeSetting)
        {
            portSetting = new ServerConfigItem<int>("Port", port);
            saveIntervalSetting = new ServerConfigItem<int>("SaveInterval", saveinterval);
            maxConnectionsSetting = new ServerConfigItem<int>("MaxConnections", maxconnection);
            disableConsoleSetting = new ServerConfigItem<bool>("DisableConsole", disableconsole);
            saveNameSetting = new ServerConfigItem<string>("SaveName", savename);
            serverPasswordSetting = new ServerConfigItem<string>("ServerPassword", serverpassword);
            adminPasswordSetting = new ServerConfigItem<string>("AdminPassword", adminpassword);
            gameModeSetting = new ServerConfigItem<string>("GameMode", gamemodeSetting.GetAttribute<DescriptionAttribute>().Description.ToString());

            //We don't want to custom those values for now
            oxygenSetting = new ServerConfigItem<float>("StartOxygen", 45);
            maxOxygenSetting = new ServerConfigItem<float>("StartMaxOxygen", 45);
            healthSetting = new ServerConfigItem<float>("StartHealth", 80);
            foodSetting = new ServerConfigItem<float>("StartFood", 50.5f);
            waterSetting = new ServerConfigItem<float>("StartWater", 90.5f);
            infectionSetting = new ServerConfigItem<float>("StartInfection", 0);
        }

        #region Properties
        public int ServerPort
        {
            get
            {
                return portSetting.Value;
            }

            set
            {
                portSetting.Value = value;
            }
        }

        public int SaveInterval
        {
            get
            {
                return saveIntervalSetting.Value;
            }

            set
            {
                saveIntervalSetting.Value = value;
            }
        }

        public int MaxConnections
        {
            get
            {
                return maxConnectionsSetting.Value;
            }

            set
            {
                maxConnectionsSetting.Value = value;
            }
        }

        public bool DisableConsole
        {
            get
            {
                return disableConsoleSetting.Value;
            }

            set
            {
                disableConsoleSetting.Value = value;
            }
        }

        public string SaveName
        {
            get
            {
                return saveNameSetting.Value;
            }

            set
            {
                saveNameSetting.Value = value;
            }
        }

        public string ServerPassword
        {
            get
            {
                return serverPasswordSetting.Value;
            }

            set
            {
                serverPasswordSetting.Value = value;
            }
        }

        public string AdminPassword
        {
            get
            {
                return adminPasswordSetting.Value;
            }

            set
            {
                adminPasswordSetting.Value = value;
            }
        }

        public string GameMode
        {
            get
            {
                return gameModeSetting.Value;
            }

            set
            {
                gameModeSetting.Value = value;
            }
        }


        public PlayerStatsData DefaultPlayerStats => new PlayerStatsData(oxygenSetting.Value, maxOxygenSetting.Value, healthSetting.Value, foodSetting.Value, waterSetting.Value, infectionSetting.Value);
        #endregion
    }
}
