using System;
using RimWorld;
using Verse;

namespace ExtractRawInsulin {
	class WorkGiver_ExtractRawInsulin : WorkGiver_GatherAnimalBodyResources{
		protected override JobDef JobDef {
			get {
				return JobDefOf.Milk;
			}
		}
		protected override CompHasGatherableBodyResource GetComp(Pawn animal) {
			return animal.TryGetComp<CompRawInsulinExtractable>();
		}
	}
}
