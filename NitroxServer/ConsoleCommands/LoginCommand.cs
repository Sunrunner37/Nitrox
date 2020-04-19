﻿using NitroxServer.ConsoleCommands.Abstract;
using NitroxModel.DataStructures.GameLogic;
using NitroxModel.DataStructures.Util;
using NitroxModel.Server;

namespace NitroxServer.ConsoleCommands
{
    internal class LoginCommand : Command
    {
        private readonly ServerConfig serverConfig;

        public LoginCommand(ServerConfig serverConfig) : base("login", Perms.PLAYER, "Log in to server as admin (requires password)")
        {
            this.serverConfig = serverConfig;
            addParameter(TypeString.Get, "password", true);
        }

        protected override void Perform(Optional<Player> sender)
        {
            string message = "Can't update permissions";

            if (sender.HasValue)
            {
                if (getArgAt(0) == serverConfig.AdminPassword)
                {
                    sender.Value.Permissions = Perms.ADMIN;
                    message = $"Updated permissions to admin for {sender.Value.Name}";
                }
                else
                {
                    message = "Incorrect Password";
                }
            }

            SendMessageToBoth(sender, message);
        }
    }
}
