﻿using NitroxModel.DataStructures.Util;
using NitroxModel.Logger;
using NitroxServer.GameLogic;
using NitroxServer.GameLogic.Bases;
using NitroxServer.GameLogic.Entities;
using NitroxServer.GameLogic.Entities.Spawning;
using NitroxServer.GameLogic.Items;
using NitroxServer.GameLogic.Players;
using NitroxServer.GameLogic.Vehicles;
using System;
using System.Collections.Generic;
using System.IO;
using NitroxServer.GameLogic.Unlockables;
using NitroxServer.ConfigParser;
using NitroxModel.DataStructures;
using NitroxModel.Core;
using NitroxModel.DataStructures.GameLogic.Entities;
using NitroxServer.GameLogic.Entities.EntityBootstrappers;
using NitroxServer.Serialization.Resources.Datastructures;
using NitroxModel.DataStructures.GameLogic;

namespace NitroxServer.Serialization.World
{
    public class WorldPersistence
    {
        private readonly ServerProtobufSerializer serializer;
        private readonly ServerConfig config;

        public WorldPersistence(ServerProtobufSerializer serializer, ServerConfig config)
        {
            this.serializer = serializer;
            this.config = config;
        }

        public void Save(World world)
        {
            try
            {
                PersistedWorldData persistedData = new PersistedWorldData();
                persistedData.ParsedBatchCells = world.BatchEntitySpawner.SerializableParsedBatches;
                persistedData.ServerStartTime = world.TimeKeeper.ServerStartTime;
                persistedData.EntityData = world.EntityData;
                persistedData.BaseData = world.BaseData;
                persistedData.VehicleData = world.VehicleData;
                persistedData.InventoryData = world.InventoryData;
                persistedData.PlayerData = world.PlayerData;
                persistedData.GameData = world.GameData;
                persistedData.EscapePodData = world.EscapePodData;

                using (Stream stream = File.OpenWrite(config.SaveName + ".nitrox"))
                {
                    serializer.SerializeVersion(stream, persistedData.version);
                    serializer.Serialize(stream, persistedData);
                }

                Log.Info("World state saved.");
            }
            catch (Exception ex)
            {
                Log.Info("Could not save world: " + ex);
            }
        }

        private Optional<World> LoadFromFile()
        {
            try
            {
                PersistedWorldData persistedData;

                using (Stream stream = File.OpenRead(config.SaveName + ".nitrox"))
                {
                    if (serializer.DeserializeVersion(stream) != PersistedWorldData.CURRENT_VERSION)
                    {
                        throw new WorldVersionMismatchException("Invalid World version!!!");
                    }
                    Stream persistedStream = new MemoryStream();
                    stream.CopyTo(persistedStream);
                    persistedStream.Position = 0;
                    using (persistedStream)
                    {
                        persistedData = serializer.Deserialize<PersistedWorldData>(stream);
                    }
                }

                if (persistedData == null || !persistedData.IsValid())
                {
                    throw new InvalidDataException("Persisted state is not valid");
                }
                

                World world = CreateWorld(persistedData.ServerStartTime,
                                          persistedData.EntityData,
                                          persistedData.BaseData,
                                          persistedData.VehicleData,
                                          persistedData.InventoryData,
                                          persistedData.PlayerData,
                                          persistedData.GameData,
                                          persistedData.ParsedBatchCells,
                                          persistedData.EscapePodData,
                                          config.GameMode);

                return Optional<World>.Of(world);
            }
            catch (FileNotFoundException)
            {
                Log.Info("No previous save file found - creating a new one.");
            }
            catch (Exception ex)
            {
                Log.Info("Could not load world: " + ex.ToString() + " creating a new one.");
            }

            return Optional<World>.Empty();
        }

        public World Load()
        {
            Optional<World> fileLoadedWorld = LoadFromFile();

            if (fileLoadedWorld.IsPresent())
            {
                return fileLoadedWorld.Get();
            }

            return CreateFreshWorld();
        }

        private World CreateFreshWorld()
        {
            World world = CreateWorld(DateTime.Now, new EntityData(), new BaseData(), new VehicleData(), new InventoryData(), new PlayerData(), new GameData() { PDAState = new PDAStateData(), StoryGoals = new StoryGoalData() }, new List<Int3>(), new EscapePodData(), config.GameMode);
            return world;
        }

        private World CreateWorld(DateTime serverStartTime,
                                  EntityData entityData,
                                  BaseData baseData,
                                  VehicleData vehicleData,
                                  InventoryData inventoryData,
                                  PlayerData playerData,
                                  GameData gameData,
                                  List<Int3> parsedBatchCells,
                                  EscapePodData escapePodData,
                                  string gameMode)
        {
            World world = new World();
            world.TimeKeeper = new TimeKeeper();
            world.TimeKeeper.ServerStartTime = serverStartTime;

            world.SimulationOwnershipData = new SimulationOwnershipData();
            world.PlayerManager = new PlayerManager(playerData, config);
            world.EntityData = entityData;
            world.EventTriggerer = new EventTriggerer(world.PlayerManager);
            world.BaseData = baseData;
            world.VehicleData = vehicleData;
            world.InventoryData = inventoryData;
            world.PlayerData = playerData;
            world.GameData = gameData;
            world.EscapePodData = escapePodData;
            world.EscapePodManager = new EscapePodManager(escapePodData);

            HashSet<TechType> serverSpawnedSimulationWhiteList = NitroxServiceLocator.LocateService<HashSet<TechType>>();
            world.EntitySimulation = new EntitySimulation(world.EntityData, world.SimulationOwnershipData, world.PlayerManager, serverSpawnedSimulationWhiteList);
            world.GameMode = gameMode;
            
            world.BatchEntitySpawner = new BatchEntitySpawner(NitroxServiceLocator.LocateService<EntitySpawnPointFactory>(),
                                                              NitroxServiceLocator.LocateService<UweWorldEntityFactory>(),
                                                              NitroxServiceLocator.LocateService<UwePrefabFactory>(),
                                                              parsedBatchCells,
                                                              serializer,
                                                              NitroxServiceLocator.LocateService<Dictionary<TechType, IEntityBootstrapper>>(),
                                                              NitroxServiceLocator.LocateService<Dictionary<string, List<PrefabAsset>>>());

            Log.Info("World GameMode: " + gameMode);

            Log.Info("Server Password: " + (string.IsNullOrEmpty(config.ServerPassword) ? "None. Public Server." : config.ServerPassword));
            Log.Info("Admin Password: " + config.AdminPassword);

            Log.Info("To get help for commands, run help in console or /help in chatbox");

            return world;
        }
    }
}
