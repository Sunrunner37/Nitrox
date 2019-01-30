﻿using System.Collections.Generic;
using NitroxClient.Communication.Abstract;
using NitroxClient.GameLogic.Helper;
using NitroxModel.DataStructures.Util;
using NitroxModel.Helper;
using NitroxModel.Logger;
using NitroxModel.Packets;
using UnityEngine;

namespace NitroxClient.GameLogic
{
    /// <summary>
    /// Handles all of the <see cref="Fire"/>s in the game. Currently, the only known Fire spawning is in <see cref="SubFire.CreateFire(SubFire.RoomFire)"/>. The
    /// fires in the Aurora come loaded with the map and do not grow in size. If we want to create a Fire spawning mechanic outside of Cyclops fires, it should be
    /// added to <see cref="Fires.Create(string, Optional{string}, Optional{CyclopsRooms}, Optional{int})"/>. Fire dousing goes by Guid and does not need to be 
    /// modified
    /// </summary>
    public class Fires
    {
        private readonly IPacketSender packetSender;

        /// <summary>
        /// Used to reduce the <see cref="FireDoused"/> packet spam as fires are being doused. A packet is only sent after
        /// the douse amount surpasses <see cref="FIRE_DOUSE_AMOUNT_TRIGGER"/>
        /// </summary>
        private readonly Dictionary<string, float> fireDouseAmount = new Dictionary<string, float>();

        /// <summary>
        /// Each extinguisher hit is from 0.15 to 0.25. 5 is a bit less than half a second of full extinguishing
        /// </summary>
        private const float FIRE_DOUSE_AMOUNT_TRIGGER = 5f;

        public Fires(IPacketSender packetSender)
        {
            this.packetSender = packetSender;
        }

        /// <summary>
        /// Triggered when <see cref="SubFire.CreateFire(SubFire.RoomFire)"/> is executed. To create a new fire manually, 
        /// call <see cref="Create(string, Optional{string}, Optional{CyclopsRooms}, Optional{int})"/>
        /// </summary>
        public void OnCreate(Fire fire, SubFire.RoomFire room, int nodeIndex)
        {
            string subRootGuid = GuidHelper.GetGuid(fire.fireSubRoot.gameObject);

            FireCreated packet = new FireCreated(GuidHelper.GetGuid(fire.gameObject), subRootGuid, room.roomLinks.room, nodeIndex);
            packetSender.Send(packet);
        }

        /// <summary>
        /// Triggered when <see cref="Fire.Douse(float)"/> is executed. To Douse a fire manually, retrieve the <see cref="Fire"/> call the Douse method
        /// </summary>
        public void OnDouse(Fire fire, float douseAmount)
        {
            string fireGuid = GuidHelper.GetGuid(fire.gameObject);

            // Temporary packet limiter
            if (!fireDouseAmount.ContainsKey(fireGuid))
            {
                fireDouseAmount.Add(fireGuid, douseAmount);
            }
            else
            {
                float summedDouseAmount = fireDouseAmount[fireGuid] + douseAmount;

                if (summedDouseAmount > FIRE_DOUSE_AMOUNT_TRIGGER)
                {
                    // It is significantly faster to keep the key as a 0 value than to remove it and re-add it later.
                    fireDouseAmount[fireGuid] = 0;

                    FireDoused packet = new FireDoused(fireGuid, douseAmount);
                    packetSender.Send(packet);
                }
            }
        }

        /// <summary>
        /// Create a new <see cref="Fire"/>. Majority of code copied from <see cref="SubFire.CreateFire(SubFire.RoomFire)"/>. Currently does not support Fires created outside of a Cyclops
        /// </summary>
        /// <param name="fireGuid">Guid of the Fire. Used for identification when dousing the fire</param>
        /// <param name="subRootGuid">Guid of the Cyclops <see cref="SubRoot"/></param>
        /// <param name="room">The room the Fire will be spawned in</param>
        /// <param name="spawnNodeIndex">Each <see cref="CyclopsRooms"/> has multiple static Fire spawn points called spawnNodes. If the wrong index is provided,
        ///     the clients will see fires in different places from the owner</param>
        public void Create(string fireGuid, string subRootGuid, CyclopsRooms room, int spawnNodeIndex)
        {
            SubFire subFire = GuidHelper.RequireObjectFrom(subRootGuid).GetComponent<SubRoot>().damageManager.subFire;
            Dictionary<CyclopsRooms, SubFire.RoomFire> roomFiresDict = (Dictionary<CyclopsRooms, SubFire.RoomFire>)subFire.ReflectionGet("roomFires");
            // Copied from SubFire_CreateFire_Patch, which copies from SubFire.CreateFire()
            Transform transform2 = roomFiresDict[room].spawnNodes[(int)spawnNodeIndex];

            // If a fire already exists at the node, replace the old Guid with the new one
            if (transform2.childCount > 0)
            {
                Fire existingFire = transform2.GetComponentInChildren<Fire>();

                if (GuidHelper.GetGuid(existingFire.gameObject) != fireGuid)
                {
                    Log.Error("[Fires.Create Fire already exists at node index " + spawnNodeIndex
                        + "! Replacing existing Fire Guid " + GuidHelper.GetGuid(existingFire.gameObject)
                        + " with Guid " + fireGuid
                        + "]");

                    GuidHelper.SetNewGuid(existingFire.gameObject, fireGuid);
                }

                return;
            }

            List<Transform> availableNodes = (List<Transform>)subFire.ReflectionGet("availableNodes");
            availableNodes.Clear();
            foreach (Transform transform in roomFiresDict[room].spawnNodes)
            {
                if (transform.childCount == 0)
                {
                    availableNodes.Add(transform);
                }
            }
            roomFiresDict[room].fireValue++;
            PrefabSpawn component = transform2.GetComponent<PrefabSpawn>();
            if (component == null)
            {
                return;
            }
            else
            {
                Log.Error("[FireCreatedProcessor Cannot create new Cyclops fire! PrefabSpawn component could not be found in fire node!"
                    + " Fire Guid: " + fireGuid
                    + " SubRoot Guid: " + subRootGuid
                    + " Room: " + room
                    + " NodeIndex: " + spawnNodeIndex
                    + "]");
            }
            GameObject gameObject = component.SpawnManual();
            Fire componentInChildren = gameObject.GetComponentInChildren<Fire>();
            if (componentInChildren)
            {
                componentInChildren.fireSubRoot = subFire.subRoot;
                GuidHelper.SetNewGuid(componentInChildren.gameObject, fireGuid);
            }

            subFire.ReflectionSet("roomFires", roomFiresDict);
            subFire.ReflectionSet("availableNodes", availableNodes);
        }
    }
}
