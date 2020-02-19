using System;
using RimWorld;
using Verse;

namespace MedicalOverhaulProject {
	class WorkGiver_ExtractRawInsulin : WorkGiver_GatherAnimalBodyResources{
		protected override JobDef JobDef {
			get {
				return DefDatabase<JobDef>.GetNamed("MOP_Job-ExtractRawInsulin", true);
			}
		}
		protected override CompHasGatherableBodyResource GetComp(Pawn animal) {
			return animal.TryGetComp<CompRawInsulinExtractable>();
		}
	}
}
