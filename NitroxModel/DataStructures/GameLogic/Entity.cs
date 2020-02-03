﻿using System;
using UnityEngine;
using System.Collections.Generic;
using ProtoBufNet;

namespace NitroxModel.DataStructures.GameLogic
{
    [Serializable]
    [ProtoContract]
    public class Entity
    {
        public AbsoluteEntityCell AbsoluteEntityCell => new AbsoluteEntityCell(Position, Level);

        [ProtoMember(1)]
        public Vector3 LocalPosition { get; set; }

        [ProtoMember(2)]
        public Quaternion LocalRotation { get; set; }

        [ProtoMember(3)]
        public Vector3 Scale { get; set; }

        [ProtoMember(4)]
        public TechType TechType { get; set; }

        [ProtoMember(5)]
        public NitroxId Id { get; set; }

        [ProtoMember(6)]
        public int Level { get; set; }

        [ProtoMember(7)]
        public string ClassId { get; set; }

        [ProtoMember(8)]
        public List<Entity> ChildEntities { get; set; } = new List<Entity>();

        [ProtoMember(9)]
        public bool SpawnedByServer; // Keeps track if an entity was spawned by the server or a player
                                     // Server-spawned entities need to be techType white-listed to be simulated

        [ProtoMember(10)]
        public NitroxId WaterParkId { get; set; }

        [ProtoMember(11)]
        public byte[] SerializedGameObject { get; set; } // Some entities (such as dropped items) have already been serialized and include 
                                                         // special game object meta data (like battery charge)
        [ProtoMember(12)]
        public bool ExistsInGlobalRoot { get; set; }

        public Entity Parent { get; set; }

        [ProtoMember(13)]
        public Vector3 Position { get; set; }

        [ProtoMember(14)]
        public Quaternion Rotation { get; set; }

        public Entity()
        {
            // Default Constructor for serialization
        }

        public Entity(Vector3 localPosition, Quaternion localRotation, Vector3 scale, TechType techType, int level, string classId, bool spawnedByServer, NitroxId id)
        {
            LocalPosition = localPosition;
            LocalRotation = localRotation;
            Scale = scale;
            TechType = techType;
            Id = id;
            Level = level;
            ClassId = classId;
            SpawnedByServer = spawnedByServer;
            WaterParkId = null;
            SerializedGameObject = null;
            ExistsInGlobalRoot = false;
            Position = Parent != null ? LocalPosition + Parent.Position : LocalPosition;
            Rotation = Parent != null ? Parent.Rotation * LocalRotation : LocalRotation;
        }

        public Entity(Vector3 position, Quaternion rotation, Vector3 scale, TechType techType, int level, string classId, bool spawnedByServer, NitroxId id, Entity parentEntity)
        {
            LocalPosition = position;
            LocalRotation = rotation;
            Scale = scale;
            TechType = techType;
            Id = id;
            Level = level;
            ClassId = classId;
            SpawnedByServer = spawnedByServer;
            WaterParkId = null;
            SerializedGameObject = null;
            ExistsInGlobalRoot = false;
            Parent = parentEntity;
            Position = Parent != null ? LocalPosition + Parent.Position : LocalPosition;
            Rotation = Parent != null ? Parent.Rotation * LocalRotation : LocalRotation;
        }

        public Entity(Vector3 position, Quaternion rotation, Vector3 scale, TechType techType, int level, string classId, bool spawnedByServer, NitroxId waterParkId, byte[] serializedGameObject, bool existsInGlobalRoot, NitroxId id)
        {
            LocalPosition = position;
            LocalRotation = rotation;
            Scale = scale;
            TechType = techType;
            Id = id;
            Level = level;
            ClassId = classId;
            SpawnedByServer = spawnedByServer;
            WaterParkId = waterParkId;
            SerializedGameObject = serializedGameObject;
            ExistsInGlobalRoot = existsInGlobalRoot;
            Position = Parent != null ? LocalPosition + Parent.Position : LocalPosition;
            Rotation = Parent != null ? Parent.Rotation * LocalRotation : LocalRotation;
        }

        public override string ToString()
        {
            return "[Entity Position: " + LocalPosition + " TechType: " + TechType + " Id: " + Id + " Level: " + Level + " classId: " + ClassId + " ChildEntities: " + string.Join(", ", ChildEntities) + " SpawnedByServer: " + SpawnedByServer + "]";
        }
    }
}
