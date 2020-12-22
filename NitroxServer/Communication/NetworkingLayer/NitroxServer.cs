using System;
using System.Collections.Generic;
using NitroxModel.DataStructures;
using NitroxModel.Logger;
using NitroxModel.Packets;
using NitroxServer.Communication.Packets;
using NitroxServer.GameLogic;
using NitroxServer.GameLogic.Entities;
using NitroxServer.Serialization;

namespace NitroxServer.Communication.NetworkingLayer
{
    public abstract class NitroxServer
    {
        protected bool isStopped = true;
        protected int portNumber, maxConn;

        protected readonly PacketHandler packetHandler;
        protected readonly EntitySimulation entitySimulation;
        protected readonly Dictionary<long, INitroxConnection> connectionsByRemoteIdentifier = new Dictionary<long, INitroxConnection>();
        protected readonly PlayerManager playerManager;

        public NitroxServer(PacketHandler packetHandler, PlayerManager playerManager, EntitySimulation entitySimulation, ServerConfig serverConfig)
        {
            this.packetHandler = packetHandler;
            this.playerManager = playerManager;
            this.entitySimulation = entitySimulation;

            portNumber = serverConfig.ServerPort;
            maxConn = serverConfig.MaxConnections;
        }

        public abstract bool Start();

        public abstract void Stop();
        
        protected void ClientDisconnected(INitroxConnection connection)
        {
            Player player = playerManager.GetPlayer(connection);

            if (player != null)
            {
                playerManager.PlayerDisconnected(connection);

                Disconnect disconnect = new Disconnect(player.Id);
                playerManager.SendPacketToAllPlayers(disconnect);

                List<SimulatedEntity> ownershipChanges = entitySimulation.CalculateSimulationChangesFromPlayerDisconnect(player);

                if (ownershipChanges.Count > 0)
                {
                    SimulationOwnershipChange ownershipChange = new SimulationOwnershipChange(ownershipChanges);
                    playerManager.SendPacketToAllPlayers(ownershipChange);
                }
            }
        }
        
        protected void ProcessIncomingData(INitroxConnection connection, Packet packet)
        {
            try
            {
                packetHandler.Process(packet, connection);
            }
            catch (Exception ex)
            {
                Log.Error("Exception while processing packet: " + packet + " " + ex);
            }
        }
    }
}
