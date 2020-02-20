using RimWorld;

using System.IO;
using Verse;
using Verse.AI;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MedicalOverhaul
{
    public class WorkGiver_BagsToBeLoaded : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get 
            {
                return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
            }
        }  

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            bool flag = pawn != null && pawn.Faction == Faction.OfPlayer;
            if (flag)
            {
                var comp = pawn.Map.GetComponent<BagsToBeLoaded>();
                if (comp != null)
                {
                    if (comp.bagsToBeLoaded != null)
                    {
                        if (comp.bagsToBeLoaded.Count > 0)
                        {
                            foreach (BagData entry in comp.bagsToBeLoaded)
                            {
                                Thing thing = GenClosest.ClosestThingReachable(entry.stand.Position, entry.stand.Map,
                                ThingRequest.ForDef(entry.bagDef), PathEndMode.OnCell,
                                TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false), 9999f,
                                null, null, 0, -1, false, RegionType.Set_Passable, false);
                                LocalTargetInfo target = thing;
                                if (pawn.CanReserveAndReach(target, PathEndMode.ClosestTouch, Danger.Deadly, 10, 1, null, forced))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            foreach (BagData entry in pawn.Map.GetComponent<BagsToBeLoaded>().bagsToBeLoaded)
            {
                Thing thing = GenClosest.ClosestThingReachable(entry.stand.Position, entry.stand.Map,
                ThingRequest.ForDef(entry.bagDef), PathEndMode.OnCell,
                TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false), 9999f,
                null, null, 0, -1, false, RegionType.Set_Passable, false);
                LocalTargetInfo target = thing;
                LocalTargetInfo target2 = (Building)entry.stand;
                if (pawn.CanReserveAndReach(target, PathEndMode.ClosestTouch, Danger.Deadly, 10, 1, null, forced))
                {
                    return new Job(DefDatabase<JobDef>.GetNamed("BagsToBeLoaded"), target2, target);
                }
            }
            return new Job(JobDefOf.Goto, pawn.Position.x + 1);
        }

    }
}