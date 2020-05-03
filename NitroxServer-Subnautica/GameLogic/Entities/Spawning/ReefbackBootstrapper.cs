﻿using NitroxModel.DataStructures;
using NitroxModel.DataStructures.GameLogic;
using NitroxModel_Subnautica.DataStructures;
using NitroxModel_Subnautica.Helper;
using NitroxServer.GameLogic.Entities.Spawning;
using static NitroxServer_Subnautica.GameLogic.Entities.Spawning.EntityBootstrappers.ReefbackSpawnData;

namespace NitroxServer_Subnautica.GameLogic.Entities.Spawning.EntityBootstrappers
{
    class ReefbackBootstrapper : IEntityBootstrapper
    {
        private float creatureProbabiltySum = 0;

        public ReefbackBootstrapper()
        {
            foreach (ReefbackEntity creature in SpawnableCreatures)
            {
                creatureProbabiltySum += creature.probability;
            }
        }

        public void Prepare(Entity parentEntity, DeterministicBatchGenerator deterministicBatchGenerator)
        {
            for(int spawnPointCounter = 0; spawnPointCounter < LocalCreatureSpawnPoints.Count; spawnPointCounter++)
            {
                Vector3 localSpawnPosition = LocalCreatureSpawnPoints[spawnPointCounter];
                float targetProbabilitySum = (float)deterministicBatchGenerator.NextDouble() * creatureProbabiltySum;
                float probabilitySum = 0;

                foreach (ReefbackEntity creature in SpawnableCreatures)
                {
                    probabilitySum += creature.probability;

                    if(probabilitySum >= targetProbabilitySum)
                    {
                        int totalToSpawn = deterministicBatchGenerator.NextInt(creature.minNumber, creature.maxNumber + 1);

                        for(int i = 0; i < totalToSpawn; i++)
                        {
                            NitroxId id = deterministicBatchGenerator.NextId();
                            Entity child = new Entity(localSpawnPosition, new Quaternion(0, 0, 0, 1), new Vector3(1, 1, 1), creature.techType.ToDto(), parentEntity.Level, creature.classId, true, id, parentEntity);
                            parentEntity.ChildEntities.Add(child);
                        }

                        break;
                    }
                }
            }
        }
    }
}
