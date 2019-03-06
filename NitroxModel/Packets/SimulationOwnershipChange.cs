﻿using System;
using System.Collections.Generic;
using System.Text;
using NitroxModel.DataStructures;

namespace NitroxModel.Packets
{
    [Serializable]
    public class SimulationOwnershipChange : Packet
    {
        public List<SimulatedEntity> Entities { get; }

        public int Count { get; set; }

        public SimulationOwnershipChange(string guid, ushort owningPlayerId, SimulationLockType lockType)
        {
            Entities = new List<SimulatedEntity>
            {
                new SimulatedEntity(guid, owningPlayerId, false, lockType)
            };
        }

        public SimulationOwnershipChange(List<SimulatedEntity> entities)
        {
            Entities = entities;
        }

        public SimulationOwnershipChange(SimulatedEntity entity)
        {
            Entities = new List<SimulatedEntity>
            {
                entity
            };
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder("[SimulationOwnershipChange - ");

            foreach (SimulatedEntity entity in Entities)
            {
                stringBuilder.Append(entity.ToString());
            }

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }
    }
}
