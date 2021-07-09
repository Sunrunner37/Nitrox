using System;
using NitroxClient.MonoBehaviours;
using NitroxClient.MonoBehaviours.Overrides;
using NitroxModel.DataStructures;
using NitroxModel.Logger;
using UnityEngine;

namespace NitroxClient.GameLogic.Bases.Spawning.Furniture
{
    /*
     * When a bio reactor is created, two objects are spawned: the main world object (BaseBioReactorGeometry) and
     * the core power logic as a separate game object (BaseBioReactor, also known as a 'module').  The BaseBioReactor 
     * resides as a direct child of the base object (probably so UWE could iterate them easy).  When the object spawns, 
     * we use this class to set a deterministic id seeded by the parent id.  This keeps inventory actions in sync.
     */
    public class BenchSpawnProcessor : FurnitureSpawnProcessor
    {
        protected override TechType[] ApplicableTechTypes { get; } =
        {
            TechType.Bench
        };

        protected override void SpawnPostProcess(GameObject finishedFurniture)
        {
            if (finishedFurniture.TryGetComponent(out Bench bench))
            {
                try
                {
                    NitroxId benchId = NitroxEntity.GetId(finishedFurniture);

                    GameObject benchTileLeft = new GameObject("BenchPlaceLeft") { layer = 13 };  // Layer 13 = Useable
                    benchTileLeft.transform.SetParent(finishedFurniture.transform, false);
                    benchTileLeft.transform.localPosition -= new Vector3(0.75f, 0, 0);
                    BoxCollider benchTileLeftCollider = benchTileLeft.AddComponent<BoxCollider>();
                    benchTileLeftCollider.center = new Vector3(0, 0.25f, 0);
                    benchTileLeftCollider.size = new Vector3(0.85f, 0.5f, 0.65f);
                    benchTileLeftCollider.isTrigger = true;

                    GameObject benchTileCenter = new GameObject("BenchPlaceCenter") { layer = 13 };
                    benchTileCenter.transform.SetParent(finishedFurniture.transform, false);
                    BoxCollider benchTileCenterCollider = benchTileCenter.AddComponent<BoxCollider>();
                    benchTileCenterCollider.center = new Vector3(0, 0.25f, 0);
                    benchTileCenterCollider.size = new Vector3(0.7f, 0.5f, 0.65f);
                    benchTileCenterCollider.isTrigger = true;

                    GameObject benchTileRight = new GameObject("BenchPlaceRight") { layer = 13 };
                    benchTileRight.transform.SetParent(finishedFurniture.transform, false);
                    benchTileRight.transform.localPosition += new Vector3(0.75f, 0, 0);
                    BoxCollider benchTileRightCollider = benchTileRight.AddComponent<BoxCollider>();
                    benchTileRightCollider.center = new Vector3(0, 0.25f, 0);
                    benchTileRightCollider.size = new Vector3(0.85f, 0.5f, 0.65f);
                    benchTileRightCollider.isTrigger = true;

                    GameObject animationRoot = finishedFurniture.FindChild("bench_animation");

                    MultiplayerBench.FromBench(bench, benchTileLeft, MultiplayerBench.Side.LEFT, animationRoot);
                    MultiplayerBench.FromBench(bench, benchTileCenter, MultiplayerBench.Side.CENTER, animationRoot);                    
                    MultiplayerBench.FromBench(bench, benchTileRight, MultiplayerBench.Side.RIGHT, animationRoot);


                    NitroxId benchLeftId = benchId.Increment();
                    NitroxId benchCenterId = benchLeftId.Increment();
                    NitroxId benchRightId = benchCenterId.Increment();
                    NitroxEntity.SetNewId(benchTileLeft, benchLeftId);
                    NitroxEntity.SetNewId(benchTileCenter, benchCenterId);
                    NitroxEntity.SetNewId(benchTileRight, benchRightId);

                    UnityEngine.Object.Destroy(bench);
                    UnityEngine.Object.Destroy(finishedFurniture.FindChild("Builder Trigger"));
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }
    }
}
