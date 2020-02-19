using System;
using RimWorld;
using Verse;

namespace MedicalOverhaulProject {
	class CompRawInsulinExtractable : CompHasGatherableBodyResource {
		
		protected override int GatherResourcesIntervalDays {
			get {
				return this.Props.rawInsulinIntervalDays;
			}
		}
		protected override int ResourceAmount {
			get {
				Random rnd = new Random();
				return rnd.Next(this.Props.rawInsulinMin, this.Props.rawInsulinMax);
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
				return (pawn==null||pawn.ageTracker.CurLifeStage.milkable);
			}
		}
		/** //Used to insta-fill a piggerino with insulin
		public override void CompTick() {
			if(this.Active) {
				float num = 1f;
				Pawn pawn = this.parent as Pawn;
				if(pawn!=null) {
					num*=PawnUtility.BodyResourceGrowthSpeed(pawn);
				}
				this.fullness+=num;
				if(this.fullness>1f) {
					this.fullness=1f;
				}
			}
		}*/
		public override string CompInspectStringExtra() {
			if(!this.Active) {
				return null;
			}
			return "ERI_RawInsulinFullness".Translate()+": "+base.Fullness.ToStringPercent();
		}
	}
}
