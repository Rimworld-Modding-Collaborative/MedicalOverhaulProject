using System;
using Verse;

namespace MedicalOverhaulProject {
	class CompProperties_RawFluidExtractable : CompProperties {
		public int rawFluidIntervalDays;
		public int rawFluidMin = 1;
		public int rawFluidMax = 1;
		public ThingDef rawFluidDef;
		public String rawFluidDefName;

		public CompProperties_RawFluidExtractable() {
			this.compClass=typeof(CompRawFluidExtractable);
		}
	}
}
