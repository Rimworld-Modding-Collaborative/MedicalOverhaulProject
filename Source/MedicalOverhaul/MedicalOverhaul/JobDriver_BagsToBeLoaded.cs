using System.Collections.Generic;

using System.IO;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
using System.Linq;

namespace MedicalOverhaul
{
    public class JobDriver_BagsToBeLoaded : JobDriver
    {
        private Building stand => TargetThingA as Building;
        private Thing bag => TargetThingB as Thing;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Pawn pawn = this.pawn;
            LocalTargetInfo target = this.stand;
            Job job = this.job;
            Log.Message("Pawn: " + pawn.Label);
            Log.Message("stand: " + stand.Label);
            Log.Message("bag: " + bag.Label);
            bool flag = !pawn.Reserve(target, job, 1, -1, null, false);
            if (!flag)
            {
                bool flag2 = pawn.Reserve(this.bag, this.job, 10, 1, null, false);
                if (flag2)
                {
                    return true;
                }
            }
            return false;
        }

        public override string GetReport()
        {
            string text = DefDatabase<JobDef>.GetNamed("BagsToBeLoaded", true).reportString;
            text = text.Replace("TargetA", TargetThingA.def.label);
            text = text.Replace("TargetB", TargetThingB.def.label);
            return text;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {

            CompRefuelable fuelType = null;
            BagData result = null;
            foreach (BagData bagData in this.pawn.Map.GetComponent<BagsToBeLoaded>().bagsToBeLoaded)
            {
                if ((bagData.stand == this.stand) && (bagData.bagDef == bag.def))
                {
                    result = bagData;
                    if (bagData.fuelType == "first")
                        fuelType = bagData.stand.firstRefuelComp;
                    else if (bagData.fuelType == "second")
                    {
                        fuelType = bagData.stand.secondRefuelComp;
                    }
                    break;
                }
            }
            if (result != null && fuelType != null)
            {
                this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
                base.AddEndCondition(delegate
                {
                    if (!fuelType.IsFull)
                    {
                        return JobCondition.Ongoing;
                    }
                    return JobCondition.Succeeded;
                });
                base.AddFailCondition(() => !this.job.playerForced && !fuelType.ShouldAutoRefuelNowIgnoringFuelPct);
                base.AddFailCondition(() => !fuelType.allowAutoRefuel && !this.job.playerForced);
                yield return Toils_General.DoAtomic(delegate
                {
                    this.job.count = fuelType.GetFuelCountToFullyRefuel();
                });
                Toil reserveFuel = Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
                yield return reserveFuel;
                yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
                yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, true, false).FailOnDestroyedNullOrForbidden(TargetIndex.B);
                yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveFuel, TargetIndex.B, TargetIndex.None, true, null);
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
                yield return Toils_General.Wait(240, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
                yield return new Toil
                {
                    initAction = delegate ()
                    {
                        Job curJob = this.pawn.CurJob;
                        Thing thing = curJob.GetTarget(TargetIndex.A).Thing;
                        if (this.pawn.CurJob.placedThings.NullOrEmpty<ThingCountClass>())
                        {
                            fuelType.Refuel(new List<Thing>
                                {
                                    curJob.GetTarget(TargetIndex.B).Thing
                                });
                            return;
                        }
                        fuelType.Refuel((from p in this.pawn.CurJob.placedThings
                                         select p.thing).ToList<Thing>());
                    },
                    defaultCompleteMode = ToilCompleteMode.Instant
                };
                if (result.fuelType == "first")
                {
                    result.stand.firstFuelType = result.bagDef;

                }
                else if (result.fuelType == "second")
                {
                    result.stand.secondFuelType = result.bagDef;
                }
                this.pawn.Map.GetComponent<BagsToBeLoaded>().bagsToBeLoaded.Remove(result);
            }
            yield break;
        }
    }
}
