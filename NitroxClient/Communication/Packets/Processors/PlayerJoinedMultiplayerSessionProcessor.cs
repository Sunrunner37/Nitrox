﻿using System.Collections.Generic;
using System.Linq;
using NitroxClient.Communication.Packets.Processors.Abstract;
using NitroxClient.GameLogic;
using NitroxModel.Packets;
using NitroxModel_Subnautica.DataStructures;

namespace NitroxClient.Communication.Packets.Processors
{
    public class PlayerJoinedMultiplayerSessionProcessor : ClientPacketProcessor<PlayerJoinedMultiplayerSession>
    {
        private readonly PlayerManager remotePlayerManager;

        public PlayerJoinedMultiplayerSessionProcessor(PlayerManager remotePlayerManager)
        {
            this.remotePlayerManager = remotePlayerManager;
        }

        public override void Process(PlayerJoinedMultiplayerSession packet)
        {
            remotePlayerManager.Create(packet.PlayerContext, packet.EquippedTechTypes.Select(techType => techType.ToUnity()));
        }
    }
}
