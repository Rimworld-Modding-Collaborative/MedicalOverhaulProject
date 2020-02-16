using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedicalOverhaul
{
	public class JobDriver_TakeProductOutOfUniversalFermenter : JobDriver
	{
		protected Thing Fermenter
		{
			get
			{
				return this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		protected Thing Product
		{
			get
			{
				return this.job.GetTarget(TargetIndex.B).Thing;
			}
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.Fermenter, this.job, 1, -1, null, true);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			CompUniversalFermenter comp = this.Fermenter.TryGetComp<CompUniversalFermenter>();
			this.FailOn(() => !comp.Fermented);
			this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
			yield return Toils_General.Wait(200, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.A).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
			yield return new Toil
			{
				initAction = delegate()
				{
					Thing thing = comp.TakeOutProduct();
					GenPlace.TryPlaceThing(thing, this.pawn.Position, this.Map, ThingPlaceMode.Near, null, null);
					StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(thing);
					IntVec3 c;
					if (StoreUtility.TryFindBestBetterStoreCellFor(thing, this.pawn, this.Map, currentPriority, this.pawn.Faction, out c, true))
					{
						this.job.SetTarget(TargetIndex.B, thing);
						this.job.count = thing.stackCount;
						this.job.SetTarget(TargetIndex.C, c);
						return;
					}
					this.EndJobWith(JobCondition.Incompletable);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
			yield return Toils_Reserve.Reserve(TargetIndex.C, 1, -1, null);
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
			yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
			Toil carry = Toils_Haul.CarryHauledThingToCell(TargetIndex.C);
			yield return carry;
			yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, carry, true);
			yield break;
		}

		private const TargetIndex FermenterInd = TargetIndex.A;

		private const TargetIndex ProductToHaulInd = TargetIndex.B;

		private const TargetIndex StorageCellInd = TargetIndex.C;

		private const int Duration = 200;
	}
}
