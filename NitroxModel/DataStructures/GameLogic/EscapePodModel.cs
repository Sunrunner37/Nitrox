﻿using System;
using System.Collections.Generic;
using ProtoBufNet;

namespace NitroxModel.DataStructures.GameLogic
{
    [Serializable]
    [ProtoContract]
    public class EscapePodModel
    {
        public const int PLAYERS_PER_ESCAPEPOD = 50;

        [ProtoMember(1)]
        public NitroxId Id { get; set; }

        [ProtoMember(2)]
        public NitroxVector3 Location { get; set; }

        [ProtoMember(3)]
        public NitroxId FabricatorId { get; set; }

        [ProtoMember(4)]
        public NitroxId MedicalFabricatorId { get; set; }

        [ProtoMember(5)]
        public NitroxId StorageContainerId { get; set; }

        [ProtoMember(6)]
        public NitroxId RadioId { get; set; }

        [ProtoMember(7)]
        public List<ushort> AssignedPlayers { get; set; } = new List<ushort>();

        [ProtoMember(8)]
        public bool Damaged { get; set; }

        [ProtoMember(9)]
        public bool RadioDamaged { get; set; }

        public void InitEscapePodModel(NitroxId id, NitroxVector3 location, NitroxId fabricatorId, NitroxId medicalFabricatorId, NitroxId storageContainerId, NitroxId radioId, bool damaged, bool radioDamaged)
        {
            Id = id;
            Location = location;
            FabricatorId = fabricatorId;
            MedicalFabricatorId = medicalFabricatorId;
            StorageContainerId = storageContainerId;
            RadioId = radioId;
            Damaged = damaged;
            RadioDamaged = radioDamaged;
        }

        public bool IsFull()
        {
            return AssignedPlayers.Count >= PLAYERS_PER_ESCAPEPOD;
        }

        public override string ToString()
        {
            return $"[EscapePodModel - Id: {Id} FabricatorId: {FabricatorId} MedicalFabricatorGuid: {MedicalFabricatorId} StorageContainerGuid: {StorageContainerId} RadioGuid: {RadioId} AssignedPlayers: {string.Join(", ", AssignedPlayers)} Damaged: {Damaged} RadioDamaged: {RadioDamaged}]";
        }
    }
}
