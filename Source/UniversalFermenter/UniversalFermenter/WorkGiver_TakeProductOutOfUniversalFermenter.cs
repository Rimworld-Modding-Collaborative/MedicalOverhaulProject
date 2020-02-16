using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace MedicalOverhaul
{
	public class WorkGiver_TakeProductOutOfUniversalFermenter : WorkGiver_Scanner
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

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			CompUniversalFermenter compUniversalFermenter = t.TryGetComp<CompUniversalFermenter>();
			return compUniversalFermenter != null && compUniversalFermenter.Fermented && !t.IsBurning() && !t.IsForbidden(pawn) && pawn.CanReserveAndReach(t, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced);
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(DefDatabase<JobDef>.GetNamed("MOP_TakeProductOutOfUniversalFermenter", true), t);
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
	}
}
