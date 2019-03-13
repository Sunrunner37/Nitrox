﻿using NitroxClient.Communication.Packets.Processors.Abstract;
using NitroxClient.GameLogic;
using NitroxModel.Packets;

namespace NitroxClient.Communication.Packets.Processors
{
    class CellEntitiesProcessor : ClientPacketProcessor<CellEntities>
    {
        private readonly Entities entities;

        public CellEntitiesProcessor(Entities entities)
        {
            this.entities = entities;
        }

        public override void Process(CellEntities packet)
        {
            NitroxModel.Logger.Log.Info("Got CellEntities Packet!: " + packet.Count);
            entities.Spawn(packet.Entities);

            SimulationOwnership.LoadedCells.Add(packet.GetDeferredCell().Get());
        }
    }
}
