﻿using System.Collections.Generic;
using NitroxModel.DataStructures.GameLogic;
using NitroxModel.DataStructures.GameLogic.Buildings.Metadata;
using NitroxModel.DataStructures;
using System.Linq;
using NitroxModel.DataStructures.Util;


namespace NitroxServer.GameLogic.Bases
{
    public class BaseManager
    {
        private Dictionary<NitroxId, BasePiece> partiallyConstructedPiecesById = new Dictionary<NitroxId, BasePiece>();
        private List<BasePiece> completedBasePieceHistory;

        public BaseManager(List<BasePiece> partiallyConstructedPieces, List<BasePiece> completedBasePieceHistory)
        {
            this.completedBasePieceHistory = completedBasePieceHistory;
            partiallyConstructedPiecesById = partiallyConstructedPieces.ToDictionary(piece => piece.Id);
        }

        public List<BasePiece> GetCompletedBasePieceHistory()
        {
            lock (completedBasePieceHistory)
            {
                return new List<BasePiece>(completedBasePieceHistory);
            }
        }

        public List<BasePiece> GetPartiallyConstructedPieces()
        {
            lock (partiallyConstructedPiecesById)
            {
                return new List<BasePiece>(partiallyConstructedPiecesById.Values);
            }
        }
        public void AddBasePiece(BasePiece basePiece)
        {
            //just in case somehow a base piece is spawned at max completion
            //the base piece will get added to at least one directory

            //playing with speed above 1 or fastbuild enabled can yeild 0% completion to 100% completion with no intermediate
            //not sure if necessary
            //UNSURE IF SAFE TO REVERT TO ORIGINAL
            if (basePiece.ConstructionAmount < 1.0f)
            {
                lock (partiallyConstructedPiecesById)
                {
                    partiallyConstructedPiecesById.Add(basePiece.Id, basePiece);
                }
                lock(completedBasePieceHistory)
                {
                    completedBasePieceHistory.Remove(basePiece);
                }
            }
            else
            {
                lock (completedBasePieceHistory)
                {
                    completedBasePieceHistory.Add(basePiece);
                }
                lock (partiallyConstructedPiecesById)
                {
                    partiallyConstructedPiecesById.Remove(basePiece.Id);
                }
            }
        }

        public void BasePieceConstructionAmountChanged(NitroxId id, float constructionAmount)
        {
            BasePiece basePiece;
            lock (partiallyConstructedPiecesById)
            {

                if (partiallyConstructedPiecesById.TryGetValue(id, out basePiece))
                {
                    basePiece.ConstructionAmount = constructionAmount;
                    if (basePiece.ConstructionCompleted)
                    {
                        basePiece.ConstructionCompleted = false;
                        //ONLY ADDS TO PARTIALLY CONSTRUCTED IF ITS NOT ALREADY IN PARTIALLY CONSTRUCTED
                        //UNSURE IF SAFE TO REMOVE
                        if (basePiece.ConstructionAmount != 1.0f && completedBasePieceHistory.Contains(basePiece) && !partiallyConstructedPiecesById.ContainsKey(basePiece.Id))
                        {
                            completedBasePieceHistory.Remove(basePiece);
                            partiallyConstructedPiecesById.Add(basePiece.Id, basePiece);
                        }
                    }
                }
            }
        }

        public void BasePieceConstructionCompleted(NitroxId id, NitroxId baseId)
        {
            BasePiece basePiece;
            lock (partiallyConstructedPiecesById)
            {
                if (partiallyConstructedPiecesById.TryGetValue(id, out basePiece))
                {
                    basePiece.ConstructionAmount = 1.0f;
                    basePiece.ConstructionCompleted = true;
                    if (!basePiece.IsFurniture)
                    {
                        // For standard base pieces, the baseId is may not be finialized until construction 
                        // completes because Subnautica uses a GhostBase in the world if there hasn't yet been
                        // a fully constructed piece.  Therefor, we always update this attribute to make sure it
                        // is the latest.
                        basePiece.BaseId = baseId;
                        basePiece.ParentId = Optional.OfNullable(baseId);
                    }
                    //NOT SURE IF THIS CAUSES ISSUES OR IS UNNECESSARY
                    //UNSURE IF SAFE TO REMOVE
                    /*
                    if (basePiece != null)
                    {
                        basePiece.BaseId = baseId;
                        basePiece.ParentId = Optional.OfNullable(baseId);
                    }
                    */
                    partiallyConstructedPiecesById.Remove(id);
                    lock (completedBasePieceHistory)
                    {
                        completedBasePieceHistory.Add(basePiece);
                    }
                }
            }
        }

        public void BasePieceDeconstructionBegin(NitroxId id)
        {
            BasePiece basePiece;

            lock (completedBasePieceHistory)
            {
                basePiece = completedBasePieceHistory.Find(piece => piece.Id == id);
                if (basePiece != null)
                {
                    basePiece.ConstructionAmount = 0.95f;
                    basePiece.ConstructionCompleted = false;
                    completedBasePieceHistory.Remove(basePiece);

                    lock (partiallyConstructedPiecesById)
                    {
                        partiallyConstructedPiecesById[basePiece.Id] = basePiece;
                    }
                    //MAYBE UNNECESSARY
                    //UNSURE IF SAFE TO REMOVE
                    if (!partiallyConstructedPiecesById.ContainsKey(basePiece.Id))
                    {
                        //MUST KEEP
                        partiallyConstructedPiecesById.Add(basePiece.Id, basePiece);
                    }
                }
            }
        }

        public void BasePieceDeconstructionCompleted(NitroxId id)
        {
            lock (partiallyConstructedPiecesById)
            {
                partiallyConstructedPiecesById.Remove(id);
            }
        }

        public void UpdateBasePieceMetadata(NitroxId id, BasePieceMetadata metadata)
        {
            BasePiece basePiece;
            lock (completedBasePieceHistory)
            {
                basePiece = completedBasePieceHistory.Find(piece => piece.Id == id);

                if (basePiece != null)
                {
                    basePiece.Metadata = Optional.OfNullable(metadata);
                }
            }
        }

        public List<BasePiece> GetBasePiecesForNewlyConnectedPlayer()
        {
            List<BasePiece> basePieces;
            List<BasePiece> CompletedPiecesHistoryOrder;
            

            lock (partiallyConstructedPiecesById)
            {
                // because base peice saving is weird I am going to comlete construction for all base peices first that did not save as completed
                // even if they were not completed before but only if they are above a certain %%
                // sometimes even base pieces were completed but dont get saved as completed so any piece over 40% will get added
                // at 100%   "BaseFoundation", "BaseRoom", "BaseMoonpool", "BaseObservatory", "BaseMapRoom", "BaseWindow", "BaseHatch", "BaseReinforcement"

                lock (completedBasePieceHistory)
                {
                    // Play back all completed base pieces first (other pieces have a dependency on these being done)
                    // I removed adding the base pieces until after the base piece "reboot"
                    basePieces = new List<BasePiece>();
                }

                CompletedPiecesHistoryOrder = new List<BasePiece>();
                //orders the base pieces correctly in basePieces
                List<string> completedBasePiecesOrder = completedBasePiecesOrder = new List<string>();
                completedBasePiecesOrder.Add("BaseFoundation");
                completedBasePiecesOrder.Add("BaseRoom");
                completedBasePiecesOrder.Add("BaseMoonpool");
                completedBasePiecesOrder.Add("BaseMapRoom");
                foreach (BasePiece completedBasePiece in completedBasePieceHistory)
                {
                    if (completedBasePiecesOrder.Contains(completedBasePiece.TechType.Name))
                    {
                        completedBasePiece.ConstructionAmount = 1.0f;
                        completedBasePiece.ConstructionCompleted = true;
                        basePieces.Add(completedBasePiece);
                        CompletedPiecesHistoryOrder.Add(completedBasePiece);
                    }
                }

                //completes the uncompleted foundations
                foreach (BasePiece partialBasePiece in partiallyConstructedPiecesById.Values)
                {
                    if (partialBasePiece.TechType.Name == "BaseFoundation"/* && partialBasePiece.ConstructionAmount > 0.4f*/)
                    {
                        // when these pieces are built I would like to remove them from partiallyConstructedPiecesById and add them to completedBasePieceHistory
                        // but threading issues pulls up an error, it seems to work without this but it could also create some other problems
                        partialBasePiece.ConstructionAmount = 1.0f;
                        partialBasePiece.ConstructionCompleted = true;
                        // not sure if this is necessary but it seems to help
                        // this adds the partial piece to the list just like the other partial pieces
                        basePieces.Add(partialBasePiece);
                        CompletedPiecesHistoryOrder.Add(partialBasePiece);
                    }
                }
                //completes the uncompleted base rooms
                foreach (BasePiece partialBasePiece in partiallyConstructedPiecesById.Values)
                {
                    if ((partialBasePiece.TechType.Name == "BaseRoom" | partialBasePiece.TechType.Name == "BaseMoonpool" | partialBasePiece.TechType.Name == "BaseMapRoom")/* && partialBasePiece.ConstructionAmount > 0f*/)
                    {
                        partialBasePiece.ConstructionAmount = 1.0f;
                        partialBasePiece.ConstructionCompleted = true;
                        basePieces.Add(partialBasePiece);
                        CompletedPiecesHistoryOrder.Add(partialBasePiece);
                    }
                }
                //completes uncompleted hatches/reinforcements/windows
                foreach (BasePiece partialBasePiece in partiallyConstructedPiecesById.Values)
                {
                    if ((partialBasePiece.TechType.Name == "BaseWindow" | partialBasePiece.TechType.Name == "BaseHatch" | partialBasePiece.TechType.Name == "BaseReinforcement")/* && partialBasePiece.ConstructionAmount > 0f*/)
                    {
                        partialBasePiece.ConstructionAmount = 1.0f;
                        partialBasePiece.ConstructionCompleted = true;
                        basePieces.Add(partialBasePiece);
                        CompletedPiecesHistoryOrder.Add(partialBasePiece);
                    }
                }
                // adds in completed base pieces after the other base peices have been fixed but avoids dupes
                foreach (BasePiece CompletedBasePiecesRemovedEdit in completedBasePieceHistory)
                {
                    if (!completedBasePiecesOrder.Contains(CompletedBasePiecesRemovedEdit.TechType.Name))
                    {
                        CompletedBasePiecesRemovedEdit.ConstructionAmount = 1.0f;
                        CompletedBasePiecesRemovedEdit.ConstructionCompleted = true;
                        basePieces.Add(CompletedBasePiecesRemovedEdit);
                        CompletedPiecesHistoryOrder.Add(CompletedBasePiecesRemovedEdit);
                    }
                }

                foreach (BasePiece partialBasePiece in partiallyConstructedPiecesById.Values)
                {
                    // security measure to make sure nothing is loaded at less than 100% 
                    // this does mean anything over 0% will be loaded to 100%
                    partialBasePiece.ConstructionAmount = 1.0f;
                    partialBasePiece.ConstructionCompleted = true;
                    if (partialBasePiece.ConstructionAmount < 0.5)
                    {
                        partialBasePiece.ConstructionAmount = 0f;
                        partialBasePiece.ConstructionCompleted = false;
                    }
                    //adds partial pieces as normal but prevents duplicates from the previous code
                    //and prevents anything less than 50% from being added
                    //that part AND partiallyConstructedPiecesById.Clear(); may or may not be able to be removed
                    if (basePieces.Contains(partialBasePiece) == false && partialBasePiece.ConstructionAmount > 0.5f)
                    {
                        basePieces.Add(partialBasePiece);
                    }
                }
            //clears partially constructed peieces bc there should be no uncompleted peices left because they have been destroyed or fully built
            //not sure if this is benificial yet...
            partiallyConstructedPiecesById.Clear();
            completedBasePieceHistory.Clear();
            completedBasePieceHistory.AddRange(CompletedPiecesHistoryOrder);
            }
            return basePieces;
        }
    }
}
