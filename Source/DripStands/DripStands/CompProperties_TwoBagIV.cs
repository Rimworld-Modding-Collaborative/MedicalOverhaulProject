using System;
using Verse;
using UnityEngine;
using RimWorld;

namespace MedicalOverhaul {
	public class CompProperties_TwoBagIV : CompProperties {
		public float bag1_Capacity = 2f;
		public float bag1_ConsumptionRate = 1f;
		public float bag1_AutoRefuelPercent;
		public bool bag1_ConsumeFuelOnlyWhenUsed = true;
		public bool bag1_targetFuelLevelConfigurable;
		public float bag1_InitialConfigurableTargetFuelLevel;
		public float bag1_MinimumFueledThreshold;
		private float bag1_Multiplier = 1f;
		public string bag1_Label;
		public string bag1_GizmoLabel;
		public string bag1_OutOfFuelMessage;
		public string bag1_IconPath;
		private Texture2D bag1_Icon;

		public float bag2_Capacity = 2f;
		public float bag2_ConsumptionRate = 1f;
		public float bag2_AutoRefuelPercent;
		public bool bag2_ConsumeFuelOnlyWhenUsed = true;
		public bool bag2_targetFuelLevelConfigurable;
		public float bag2_InitialConfigurableTargetFuelLevel;
		public float bag2_MinimumFueledThreshold;
		private float bag2_Multiplier = 1f;
		public string bag2_Label;
		public string bag2_GizmoLabel;
		public string bag2_OutOfFuelMessage;
		public string bag2_IconPath;
		private Texture2D bag2_Icon;

		public ThingFilter bagFilter;
		public bool drawFuelGaugeInMap;
		public bool drawOutOfFuelOverlay = true;
		public bool showFuelGizmo = true;
		public bool shouldAllowAutoRefuelToggle = true;
		public bool factorByDifficulty;
		public bool initialAllowAutoRefuel = true;

		public CompProperties_TwoBagIV() {
			this.compClass=typeof(Comp_TwoBagIV);
		}
		public string Bag1_Label {
			get {
				if(this.bag1_Label.NullOrEmpty()) {
					return "bag 1".TranslateSimple();
				}
				return this.bag1_Label;
			}
		}
		public string Bag2_Label {
			get {
				if(this.bag2_Label.NullOrEmpty()) {
					return "bag 2".TranslateSimple();
				}
				return this.bag2_Label;
			}
		}

		public string Bag1_GizmoLabel {
			get {
				if(this.bag1_GizmoLabel.NullOrEmpty()) {
					return "bag 1".TranslateSimple();
				}
				return this.bag1_GizmoLabel;
			}
		}
		public string Bag2_GizmoLabel {
			get {
				if(this.bag2_GizmoLabel.NullOrEmpty()) {
					return "bag 2".TranslateSimple();
				}
				return this.bag2_GizmoLabel;
			}
		}

		public Texture2D Bag1_Icon {
			get {
				if(this.bag1_Icon==null) {
					if(!this.bag1_IconPath.NullOrEmpty()) {
						this.bag1_Icon=ContentFinder<Texture2D>.Get(this.bag1_IconPath, true);
					}
					else {
						ThingDef thingDef;
						if(this.bagFilter.AnyAllowedDef!=null) {
							thingDef=this.bagFilter.AnyAllowedDef;
						}
						else {
							thingDef=ThingDefOf.Chemfuel;
						}
						this.bag1_Icon=thingDef.uiIcon;
					}
				}
				return this.bag1_Icon;
			}
		}
		public Texture2D Bag2_Icon {
			get {
				if(this.bag2_Icon==null) {
					if(!this.bag2_IconPath.NullOrEmpty()) {
						this.bag2_Icon=ContentFinder<Texture2D>.Get(this.bag2_IconPath, true);
					}
					else {
						ThingDef thingDef;
						if(this.bagFilter.AnyAllowedDef!=null) {
							thingDef=this.bagFilter.AnyAllowedDef;
						}
						else {
							thingDef=ThingDefOf.Chemfuel;
						}
						this.bag2_Icon=thingDef.uiIcon;
					}
				}
				return this.bag2_Icon;
			}
		}
		public float bag1_MultiplierCurrentDifficulty {
			get {
				if(this.factorByDifficulty) {
					return this.bag1_Multiplier/Find.Storyteller.difficulty.maintenanceCostFactor;
				}
				return this.bag1_Multiplier;
			}
		}
		public float bag2_MultiplierCurrentDifficulty {
			get {
				if(this.factorByDifficulty) {
					return this.bag2_Multiplier/Find.Storyteller.difficulty.maintenanceCostFactor;
				}
				return this.bag2_Multiplier;
			}
		}

		public override void ResolveReferences(ThingDef parentDef) {
			base.ResolveReferences(parentDef);
			this.bagFilter.ResolveReferences();
		}
	}
}
