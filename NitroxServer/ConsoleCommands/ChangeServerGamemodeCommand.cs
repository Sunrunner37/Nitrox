﻿using NitroxModel.DataStructures.GameLogic;
using NitroxModel.Packets;
using NitroxModel.Serialization;
using NitroxModel.Server;
using NitroxServer.ConsoleCommands.Abstract;
using NitroxServer.ConsoleCommands.Abstract.Type;
using NitroxServer.GameLogic;
using NitroxServer.Serialization;

namespace NitroxServer.ConsoleCommands
{
    internal class ChangeServerGamemodeCommand : Command
    {
        private readonly PlayerManager playerManager;
        private readonly ServerConfig serverConfig;

        public ChangeServerGamemodeCommand(PlayerManager playerManager, ServerConfig serverConfig) : base("changeservergamemode", Perms.ADMIN, "Changes server gamemode")
        {
            AddParameter(new TypeEnum<ServerGameMode>("gamemode", true));
            this.playerManager = playerManager;
            this.serverConfig = serverConfig;
        }

        protected override void Execute(CallArgs args)
        {
            ServerGameMode sgm = args.Get<ServerGameMode>(0);

            serverConfig.GameMode = sgm;
            NitroxConfig.Serialize(serverConfig);

            playerManager.SendPacketToAllPlayers(new GameModeChanged(sgm));

            SendMessageToAllPlayers($"Server gamemode changed to \"{sgm}\" by {args.SenderName}");
        }
    }
}
