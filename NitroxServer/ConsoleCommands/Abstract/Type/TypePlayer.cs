﻿using NitroxModel.Core;
using NitroxModel.Helper;
using NitroxServer.Exceptions;
using NitroxServer.GameLogic;

namespace NitroxServer.ConsoleCommands.Abstract.Type
{
    public class TypePlayer : Parameter<Player>
    {
        private static readonly PlayerManager playerManager = NitroxServiceLocator.LocateService<PlayerManager>();

        public TypePlayer(string name, bool required) : base(name, required)
        {
            Validate.NotNull(playerManager, "PlayerManager can't be null to resolve the command");
        }

        public override bool IsValid(string arg)
        {
            Player player;
            return playerManager.TryGetPlayerByName(arg, out player);
        }

        public override Player Read(string arg)
        {
            Player player;

            if (!playerManager.TryGetPlayerByName(arg, out player))
            {
                throw new IllegalArgumentException("Player not found");
            }

            return player;
        }
    }
}
