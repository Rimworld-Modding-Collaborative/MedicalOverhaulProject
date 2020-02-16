using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MedicalOverhaul
{
	public class JobDriver_FillUniversalFermenter : JobDriver
	{
		protected Thing Fermenter
		{
			get
			{
				return this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		protected Thing Ingredient
		{
			get
			{
				return this.job.GetTarget(TargetIndex.B).Thing;
			}
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.Fermenter, this.job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(this.Ingredient, this.job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			CompUniversalFermenter comp = this.Fermenter.TryGetComp<CompUniversalFermenter>();
			this.FailOn(() => comp.SpaceLeftForIngredient <= 0);
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnBurningImmobile(TargetIndex.A);
			this.FailOnDestroyedNullOrForbidden(TargetIndex.B);
			Toil ingrToil = Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
			yield return ingrToil;
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.B);
			yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, true, false).FailOnDestroyedNullOrForbidden(TargetIndex.B);
			yield return Toils_Haul.CheckForGetOpportunityDuplicate(ingrToil, TargetIndex.B, TargetIndex.None, true, null);
			yield return Toils_Haul.CarryHauledThingToCell(TargetIndex.A);
			yield return Toils_General.Wait(200, TargetIndex.None).FailOnDestroyedNullOrForbidden(TargetIndex.A).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
			yield return new Toil
			{
				initAction = delegate()
				{
					if (!comp.AddIngredient(this.Ingredient))
					{
						this.EndJobWith(JobCondition.Incompletable);
						Log.Message("JobCondition.Incompletable", false);
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield break;
		}

		private const TargetIndex FermenterInd = TargetIndex.A;

		private const TargetIndex IngredientInd = TargetIndex.B;

		private const int Duration = 200;
	}
}
