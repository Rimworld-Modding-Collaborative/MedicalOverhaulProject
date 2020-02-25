using System;
using RimWorld;
using Verse;
using UnityEngine;

namespace MedicalOverhaul {
	[StaticConstructorOnStartup]
	public class Comp_TwoBagIV : ThingComp {
		public ThingDef bag1;
		public ThingDef bag2;
		private float bag1_level;
		private float bag2_level;

		public bool allowAutoRefill = true;

		public const string refueledSignal = "Refueled";
		public const string ranOutOfFuelSignal = "RanOutOfFuel";

		private static readonly Vector2 BarSize = new Vector2(1f, 0.2f);
		private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.6f, 0.56f, 0.13f), false);
		private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);

		private CompFlickable flickComp;
		private float bag1_configuredTargetLevel = -1f;
		private float bag2_configuredTargetLevel = -1f;

		public CompProperties_TwoBagIV Props {
			get {return (CompProperties_TwoBagIV)this.props;}
		}

		public ThingDef Bag1 {
			get { return this.bag1;}
			set { this.bag1=value; }
		}
		public ThingDef Bag2 {
			get { return this.bag2; }
			set { this.bag2=value; }
		}
		public float Bag1_TargetLevel {
			get {
				if(this.bag1_configuredTargetLevel>=0f) {
					return this.bag1_configuredTargetLevel;
				}
				if(this.Props.bag1_targetFuelLevelConfigurable) {
					return this.Props.bag1_InitialConfigurableTargetFuelLevel;
				}
				return this.Props.bag1_Capacity;
			}
			set {
				this.bag1_configuredTargetLevel=Mathf.Clamp(value, 0f, this.Props.bag1_Capacity);
			}
		}
		public float Bag2_TargetLevel {
			get {
				if(this.bag2_configuredTargetLevel>=0f) {
					return this.bag2_configuredTargetLevel;
				}
				if(this.Props.bag2_targetFuelLevelConfigurable) {
					return this.Props.bag2_InitialConfigurableTargetFuelLevel;
				}
				return this.Props.bag2_Capacity;
			}
			set {
				this.bag2_configuredTargetLevel=Mathf.Clamp(value, 0f, this.Props.bag2_Capacity);
			}
		}
		public float Bag1_Level {
			get {return this.bag1_level;}
		}
		public float Bag2_Level {
			get {return this.bag2_level;}
			
		}

		public float Bag1_PercentOfTarget {
			get {return this.bag1_level/this.Bag1_TargetLevel;}
		}
		public float Bag2_PercentOfTarget {
			get { return this.bag2_level/this.Bag2_TargetLevel; }
		}
		public float Bag1_PercentOfMax {
			get {return this.bag1_level/this.Props.bag1_Capacity;}
		}
		public float Bag2_PercentOfMax {
			get { return this.bag2_level/this.Props.bag2_Capacity; }
		}
		public bool Bag1_IsFull {
			get {return this.Bag1_TargetLevel-this.bag1_level<1f;}
		}
		public bool Bag2_IsFull {
			get {return this.Bag2_TargetLevel-this.bag2_level<1f;}
		}
		public bool Bag1_HasFuel {
			get {return this.bag1_level>0f&&this.bag1_level>=this.Props.bag1_MinimumFueledThreshold;}
		}
		public bool Bag2_HasFuel {
			get { return this.bag2_level>0f&&this.bag2_level>=this.Props.bag2_MinimumFueledThreshold; }
		}
		private float Bag1_ConsumptionRatePerTick {
			get {return this.Props.bag1_ConsumptionRate/60000f;}
		}
		private float Bag2_ConsumptionRatePerTick {
			get { return this.Props.bag2_ConsumptionRate/60000f; }
		}
		public bool Bag1_ShouldAutoRefuelNow {
			get {
				return this.Bag1_PercentOfTarget<=this.Props.bag1_AutoRefuelPercent&&!this.Bag1_IsFull&&this.Bag1_TargetLevel>0f&&this.ShouldAutoRefuelNowIgnoringFuelPct;
			}
		}
		public bool Bag2_ShouldAutoRefuelNow {
			get {
				return this.Bag2_PercentOfTarget<=this.Props.bag2_AutoRefuelPercent&&!this.Bag2_IsFull&&this.Bag2_TargetLevel>0f&&this.ShouldAutoRefuelNowIgnoringFuelPct;
			}
		}
		public bool ShouldAutoRefuelNowIgnoringFuelPct {
			get {
				return !this.parent.IsBurning()&&(this.flickComp==null||this.flickComp.SwitchIsOn)&&this.parent.Map.designationManager.DesignationOn(this.parent, DesignationDefOf.Flick)==null&&this.parent.Map.designationManager.DesignationOn(this.parent, DesignationDefOf.Deconstruct)==null;
			}
		}
		public override void Initialize(CompProperties props) {
			base.Initialize(props);
			this.allowAutoRefill=this.Props.initialAllowAutoRefuel;
			this.bag1_level=0;
			this.bag2_level=0;
			this.flickComp=this.parent.GetComp<CompFlickable>();
		}
		public override void PostExposeData() {
			base.PostExposeData();
			Scribe_Values.Look<float>(ref this.bag1_level, "bag1", 0f, false);
			Scribe_Values.Look<float>(ref this.bag2_level, "bag2", 0f, false);
			Scribe_Values.Look<float>(ref this.bag1_configuredTargetLevel, "bag1_configuredTargetLevel", -1f, false);
			Scribe_Values.Look<float>(ref this.bag2_configuredTargetLevel, "bag2_configuredTargetLevel", -1f, false);
			Scribe_Values.Look<bool>(ref this.allowAutoRefill, "allowAutoRefill", false, false);
		}

		public override void PostDraw() {
			base.PostDraw();
			if(!this.allowAutoRefill) {
				this.parent.Map.overlayDrawer.DrawOverlay(this.parent, OverlayTypes.ForbiddenRefuel);
			}
			else {
				if(!this.Bag1_HasFuel&&this.Props.drawOutOfFuelOverlay) {
					this.parent.Map.overlayDrawer.DrawOverlay(this.parent, OverlayTypes.OutOfFuel);
				}
			}

			if(this.Props.drawFuelGaugeInMap) {
				GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
				r.center=this.parent.DrawPos+Vector3.up*0.2f;
				r.size=Comp_TwoBagIV.BarSize;
				r.fillPercent=this.Bag1_PercentOfMax;
				r.filledMat=Comp_TwoBagIV.BarFilledMat;
				r.unfilledMat=Comp_TwoBagIV.BarUnfilledMat;
				r.margin=0.15f;
				Rot4 rotation = this.parent.Rotation;
				rotation.Rotate(RotationDirection.Clockwise);
				r.rotation=rotation;
				GenDraw.DrawFillableBar(r);
				
				GenDraw.FillableBarRequest q = default(GenDraw.FillableBarRequest);
				q.center=this.parent.DrawPos+Vector3.up*0.1f;
				q.size=Comp_TwoBagIV.BarSize;
				q.fillPercent=this.Bag2_PercentOfMax;
				q.filledMat=Comp_TwoBagIV.BarFilledMat;
				q.unfilledMat=Comp_TwoBagIV.BarUnfilledMat;
				q.margin=0.15f;
				q.rotation=rotation;
				GenDraw.DrawFillableBar(q);
			}
		}
			public void ConsumeFuel(float amount) {
			if(this.bag1_level<=0f || this.bag2_level<=0f) {
				return;
			}
			this.bag1_level-=amount;
			this.bag2_level-=amount;
			if(this.bag1_level<=0f) {
				this.bag1_level=0f;
				this.parent.BroadcastCompSignal("RanOutOfFuel");
			}
			if(this.bag2_level<=0f) {
				this.bag2_level=0f;
				this.parent.BroadcastCompSignal("RanOutOfFuel");
			}
		}
	}
}
