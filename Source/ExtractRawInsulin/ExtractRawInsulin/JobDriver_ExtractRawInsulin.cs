using System;
using Verse;
using RimWorld;

namespace ExtractRawInsulin {
	class JobDriver_ExtractRawInsulin : JobDriver_GatherAnimalBodyResources {
		protected override float WorkTotal {
			get {
				return 400f;
			}
		}
		protected override CompHasGatherableBodyResource GetComp(Pawn animal) {
			return animal.TryGetComp<CompRawInsulinExtractable>();
		}
	}
}
