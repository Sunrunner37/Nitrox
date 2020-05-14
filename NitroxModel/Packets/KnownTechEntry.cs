﻿using System;
using NitroxModel.DataStructures;

namespace NitroxModel.Packets
{
    [Serializable]
    public class KnownTechEntryAdd : Packet
    {
        public TechType TechType { get; }
        public bool Verbose { get; }

        public KnownTechEntryAdd(TechType techType, bool verbose)
        {
            TechType = techType;
            Verbose = verbose;
        }
        public override string ToString()
        {
            return "[KnownTechEntryAdd - TechType: " + TechType + " Verbose: " + Verbose + "]";
        }
    }    
}
