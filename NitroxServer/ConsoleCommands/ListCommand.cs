﻿using NitroxServer.ConsoleCommands.Abstract;
using NitroxServer.GameLogic;
using System.Collections.Generic;
using System.Linq;
using NitroxModel.DataStructures.GameLogic;
using NitroxModel.DataStructures.Util;
using NitroxModel.Logger;

namespace NitroxServer.ConsoleCommands
{
    internal class ListCommand : Command
    {
        private readonly PlayerManager playerManager;

        public ListCommand(PlayerManager playerManager) : base("list", Perms.PLAYER, "Shows who's online")
        {
            this.playerManager = playerManager;
        }

        protected override void Execute(Optional<Player> sender)
        {
            List<Player> players = playerManager.GetConnectedPlayers();
            string playerList = "List of players : " + string.Join(", ", players);

            if (players.Count == 0)
            {
                playerList += "No players online";
            }

            SendMessage(sender, playerList);
        }
    }
}
