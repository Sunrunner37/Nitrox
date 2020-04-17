﻿using NitroxClient.Communication.Abstract;
using NitroxClient.Communication.Packets.Processors.Abstract;
using NitroxClient.MonoBehaviours;
using NitroxModel.DataStructures.Util;
using NitroxModel.Logger;
using UnityEngine;

namespace NitroxClient.Communication.Packets.Processors
{
    class PrecursorDoorToggleProcessor : ClientPacketProcessor<NitroxModel.Packets.PrecursorDoorwayAction>
    {
        private readonly IPacketSender packetSender;

        public PrecursorDoorToggleProcessor(IPacketSender packetSender)
        {
            this.packetSender = packetSender;
        }

        public override void Process(NitroxModel.Packets.PrecursorDoorwayAction packet)
        {
            Optional<GameObject> gameObject = NitroxEntity.GetObjectFrom(packet.Id);
            if (!gameObject.HasValue)
            {
                Log.Error($"Precursor door not found by nitrox id {packet.Id}");
                return;
            }
            PrecursorDoorway precursorDoorway = gameObject.Value.GetComponent<PrecursorDoorway>();
            if (!precursorDoorway)
            {
                Log.Error($"Precursor door component not found by nitrox id {packet.Id}");
                return;
            }
            if (packet.isOpen != precursorDoorway.isOpen)
            {
                using (packetSender.Suppress<NitroxModel.Packets.PrecursorDoorwayAction>())
                {
                    precursorDoorway.ToggleDoor(packet.isOpen);
                }
            }
        }
    }
}
