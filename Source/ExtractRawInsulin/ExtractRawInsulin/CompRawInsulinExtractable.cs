using System;
using RimWorld;
using Verse;

namespace ExtractRawInsulin {
	class CompRawInsulinExtractable : CompHasGatherableBodyResource {
		protected override int GatherResourcesIntervalDays {
			get {
				return this.Props.rawInsulinIntervalDays;
			}
		}
		protected override int ResourceAmount {
			get {
				return this.Props.rawInsulinAmount;
			}
		}
		protected override ThingDef ResourceDef {
			get {
				return this.Props.rawInsulinDef;
			}
		}
		protected override string SaveKey {
			get {
				return "eri_rawInsulinFullness";
			}
		}
		public CompProperties_RawInsulinExtractable Props {
			get {
				return (CompProperties_RawInsulinExtractable)this.props;
			}
		}
		protected override bool Active {
			get {
				if(!base.Active) {
					return false;
				}
				Pawn pawn = this.parent as Pawn;
				return (!this.Props.extractFemaleOnly||pawn==null||pawn.gender==Gender.Female)&&(pawn==null||pawn.ageTracker.CurLifeStage.milkable);
			}
		}
		public override string CompInspectStringExtra() {
			if(!this.Active) {
				return null;
			}
			return "ERI_RawInsulinFullness".Translate()+": "+base.Fullness.ToStringPercent();
		}
	}
}
