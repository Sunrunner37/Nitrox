﻿using NitroxModel.DataStructures;
using NitroxModel.GameLogic;
using NitroxModel.Logger;
using NitroxModel.Packets;
using NitroxServer.Communication.Packets.Processors.Abstract;
using NitroxServer.GameLogic;
using System;
using System.Collections.Generic;

namespace NitroxServer.Communication.Packets.Processors
{
    class CellVisibilityChangedProcessor : AuthenticatedPacketProcessor<CellVisibilityChanged>
    {
        private readonly EntityManager entityManager;
        private readonly PlayerManager playerManager;

        public CellVisibilityChangedProcessor(EntityManager entityManager, PlayerManager playerManager)
        {            
            this.entityManager = entityManager;
            this.playerManager = playerManager;
        }
        
        public override void Process(CellVisibilityChanged packet, Player player)
        {
            player.AddCells(packet.Added);
            player.RemoveCells(packet.Removed);

            List<OwnedGuid> ownershipChanges = new List<OwnedGuid>();
            AssignLoadedCellEntitySimulation(player.Id, packet.Added, ownershipChanges);
            ReassignRemovedCellEntitySimulation(player, packet.Removed, ownershipChanges);
            BroadcastSimulationChanges(ownershipChanges);

            SendNewlyVisibleEntities(player, packet.Added);
        }
        
        private void SendNewlyVisibleEntities(Player player, VisibleCell[] visibleCells)
        {
            List<Entity> newlyVisibleEntities = entityManager.GetVisibleEntities(visibleCells);

            if (newlyVisibleEntities.Count > 0)
            {
                CellEntities cellEntities = new CellEntities(newlyVisibleEntities);
                player.SendPacket(cellEntities);
            }
        }

        private void AssignLoadedCellEntitySimulation(String playerId, VisibleCell[] addedCells, List<OwnedGuid> ownershipChanges)
        {
            List<Entity> entities = entityManager.AssignEntitySimulation(playerId, addedCells);

            foreach (Entity entity in entities)
            {
                ownershipChanges.Add(new OwnedGuid(entity.Guid, playerId, true));
            }
        }

        private void ReassignRemovedCellEntitySimulation(Player sendingPlayer, VisibleCell[] removedCells, List<OwnedGuid> ownershipChanges)
        {
            List<Entity> revokedEntities = entityManager.RevokeEntitySimulationFor(sendingPlayer.Id, removedCells);
            
            foreach (Entity entity in revokedEntities)
            {
                VisibleCell entityCell = new VisibleCell(entity.Position, entity.Level);

                foreach (Player player in playerManager.GetPlayers())
                {
                    if (player != sendingPlayer && player.HasCellLoaded(entityCell))
                    {
                        Log.Info("player " + player.Id + " can take over " + entity.Guid);
                        ownershipChanges.Add(new OwnedGuid(entity.Guid, player.Id, true));
                    }
                }
            }
        }

        private void BroadcastSimulationChanges(List<OwnedGuid> ownershipChanges)
        {
            if (ownershipChanges.Count > 0)
            {
                SimulationOwnershipChange ownershipChange = new SimulationOwnershipChange(ownershipChanges);
                playerManager.SendPacketToAllPlayers(ownershipChange);
            }
        }
    }
}
