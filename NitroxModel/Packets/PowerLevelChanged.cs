﻿using System;
using NitroxModel.DataStructures.GameLogic;

namespace NitroxModel.Packets
{
    [Serializable]
    public class PowerLevelChanged : AuthenticatedPacket
    {
        public String Guid { get; }
        public float Amount { get; }
        public PowerType PowerType { get; }

        public PowerLevelChanged(String playerId, String guid, float amount, PowerType powerType) : base(playerId)
        {
            Guid = guid;
            Amount = amount;
            PowerType = powerType;
        }

        public override string ToString()
        {
            return "[PowerLevelChanged - playerId: " + PlayerId + " Amount: " + Amount + " PowerType: " + PowerType + " guid: " + Guid + "]";
        }
    }
}
