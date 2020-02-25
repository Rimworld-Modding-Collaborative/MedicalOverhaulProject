using System;
using Verse;
using RimWorld;

namespace MedicalOverhaul {
	class JobDriver_ExtractRawFluid : JobDriver_GatherAnimalBodyResources {
		protected override float WorkTotal {
			get {
				return 400f;
			}
		}
		protected override CompHasGatherableBodyResource GetComp(Pawn animal) {
			return animal.TryGetComp<CompRawFluidExtractable>();
		}
	}
}
