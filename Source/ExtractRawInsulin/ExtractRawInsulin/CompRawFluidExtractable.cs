using System;
using RimWorld;
using Verse;

namespace MedicalOverhaulProject {
	class CompRawFluidExtractable : CompHasGatherableBodyResource {
		
		protected override int GatherResourcesIntervalDays {
			get {
				return this.Props.rawFluidIntervalDays;
			}
		}
		protected override int ResourceAmount {
			get {
				Random rnd = new Random();
				return rnd.Next(this.Props.rawFluidMin, this.Props.rawFluidMax);
			}
		}
		protected override ThingDef ResourceDef {
			get {
				return this.Props.rawFluidDef;
			}
		}
		protected override string SaveKey {
			get {
				return "erf_"+this.Props.rawFluidDefName+"_Fullness";
			}
		}
		public CompProperties_RawFluidExtractable Props {
			get {
				return (CompProperties_RawFluidExtractable)this.props;
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
				float num = .005f;
				Pawn pawn = this.parent as Pawn;
				if(pawn!=null) {
					num*=PawnUtility.BodyResourceGrowthSpeed(pawn);
				}
				this.fullness+=num;
				if(this.fullness>1f) {
					this.fullness=1f;
				}
			}
		}
	    //*/
		public override string CompInspectStringExtra() {
			if(!this.Active) {
				return null;
			}
			return ("ERF_"+this.Props.rawFluidDefName+"_Fullness").Translate()+": "+base.Fullness.ToStringPercent();
		}
	}
}
