﻿using System;
using NitroxServer.GameLogic.Unlockables;
using ProtoBufNet;

namespace NitroxServer.GameLogic
{
    [Serializable]
    [ProtoContract]
    public class GameData
    {
        [ProtoMember(1)]
        public PDAStateData PDAState { get; set; }
        
        [ProtoMember(2)]
        public StoryGoalData StoryGoals { get; set; }
        
        [ProtoMember(3)]
        public StoryTimingData StoryTiming { get; set; }
    }
}
