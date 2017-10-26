﻿using System;
using UnityEngine;

namespace NitroxModel.Packets
{
    [Serializable]
    public class CyclopsChangeColor : AuthenticatedPacket
    {
        public String Guid { get; }
        public int Index { get; }
        public Vector3 HSB { get; }
        public Color Color { get; }

        public CyclopsChangeColor(String playerId, int index, String guid, Vector3 hsb, Color color) : base(playerId)
        {
            Guid = guid;
            Index = index;
            HSB = hsb;
            Color = color;
        }

        public override string ToString()
        {
            return "[CyclopsChangeColor PlayerId: " + PlayerId + " Guid: " + Guid + " Index: " + Index + " hsb: " + HSB + " Color: " + Color + "]";
        }
    }
}
