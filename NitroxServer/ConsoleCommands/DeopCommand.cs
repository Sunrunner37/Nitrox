﻿using NitroxServer.ConsoleCommands.Abstract;
using NitroxModel.DataStructures.GameLogic;
using NitroxServer.GameLogic;
using NitroxModel.DataStructures.Util;

namespace NitroxServer.ConsoleCommands
{
    internal class DeopCommand : Command
    {
        private readonly PlayerManager playerManager;

        public DeopCommand(PlayerManager playerManager) : base("deop", Perms.ADMIN, "Removes admin rights from user")
        {
            this.playerManager = playerManager;
            addParameter(TypePlayer.Get, "name", true);
        }

        protected override void Perform(Optional<Player> sender)
        {
            Player targetPlayer = readArgAt(0);
            string playerName = getArgAt(0);
            string message;

            if (targetPlayer != null)
            {
                targetPlayer.Permissions = Perms.PLAYER;
                message = $"Updated {playerName}\'s permissions to PLAYER";
            }
            else
            {
                message = $"Could not update permissions of unknown player {playerName}";
            }

            SendMessageToBoth(sender, message);
        }
    }
}
