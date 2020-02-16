using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedicalOverhaul
{
	public class WorkGiver_FillUniversalFermenter : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Undefined);
			}
		}

		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		public static void Reset()
		{
			WorkGiver_FillUniversalFermenter.TemperatureTrans = "BadTemperature".Translate().ToLower();
			WorkGiver_FillUniversalFermenter.NoIngredientTrans = "UF_NoIngredient".Translate();
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			CompUniversalFermenter compUniversalFermenter = t.TryGetComp<CompUniversalFermenter>();
			if (compUniversalFermenter == null || compUniversalFermenter.Fermented || compUniversalFermenter.SpaceLeftForIngredient <= 0)
			{
				return false;
			}
			float ambientTemperature = compUniversalFermenter.parent.AmbientTemperature;
			if (ambientTemperature < compUniversalFermenter.Product.temperatureSafe.min + 2f || ambientTemperature > compUniversalFermenter.Product.temperatureSafe.max - 2f)
			{
				JobFailReason.Is(WorkGiver_FillUniversalFermenter.TemperatureTrans, null);
				return false;
			}
			if (t.IsForbidden(pawn) || !pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced))
			{
				return false;
			}
			if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
			{
				return false;
			}
			if (this.FindIngredient(pawn, t) == null)
			{
				JobFailReason.Is(WorkGiver_FillUniversalFermenter.NoIngredientTrans, null);
				return false;
			}
			return !t.IsBurning();
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Thing t2 = this.FindIngredient(pawn, t);
			return new Job(DefDatabase<JobDef>.GetNamed("MOP_FillUniversalFermenter", true), t, t2)
			{
				count = t.TryGetComp<CompUniversalFermenter>().SpaceLeftForIngredient
			};
		}

		private Thing FindIngredient(Pawn pawn, Thing fermenter)
		{
			ThingFilter filter = fermenter.TryGetComp<CompUniversalFermenter>().Product.ingredientFilter;
			Predicate<Thing> validator = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, -1, null, false) && filter.Allows(x);
			return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, filter.BestThingRequest, PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
		}
         
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            List<Thing>.Enumerator enumerator = default(List<Thing>.Enumerator);
            foreach (Thing thing2 in pawn.Map.listerThings.ThingsOfDef(ThingDef.Named("MOP_DeepFermentationTank")))
            {
                yield return thing2;
            }
            enumerator = default(List<Thing>.Enumerator);
            yield break;
            yield break;
        }
		private static string TemperatureTrans;

		private static string NoIngredientTrans;
	}
}
