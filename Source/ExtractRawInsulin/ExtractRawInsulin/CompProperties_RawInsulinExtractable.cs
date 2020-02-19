using System;
using Verse;

namespace MedicalOverhaulProject {
	class CompProperties_RawInsulinExtractable : CompProperties {
		public int rawInsulinIntervalDays;
		public int rawInsulinAmount = 1;
		public ThingDef rawInsulinDef;
		public bool extractFemaleOnly = false;

		public CompProperties_RawInsulinExtractable() {
			this.compClass=typeof(CompRawInsulinExtractable);
		}
	}
}
