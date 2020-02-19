using System;
using Verse;

namespace MedicalOverhaulProject {
	class CompProperties_RawInsulinExtractable : CompProperties {
		public int rawInsulinIntervalDays;
		public int rawInsulinMin = 1;
		public int rawInsulinMax = 1;
		public ThingDef rawInsulinDef;

		public CompProperties_RawInsulinExtractable() {
			this.compClass=typeof(CompRawInsulinExtractable);
		}
	}
}
