using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Reflection;

namespace MedicalOverhaul {
	public class IV_Stand : Building {
		public IV_Stand() {}

		public override void SpawnSetup(Map map, bool respawningAfterLoad) {
			base.SpawnSetup(map, respawningAfterLoad);
			this.firstRefuelComp=base.GetComp<CompRefuelable>();
			this.secondRefuelComp=new CompRefuelable();
			this.firstRefuelComp=base.GetComp<CompRefuelable>();
			CompProperties_Refuelable compProperties_Refuelable = new CompProperties_Refuelable {
				consumeFuelOnlyWhenUsed=true,
				drawOutOfFuelOverlay=false,
				drawFuelGaugeInMap=false,
				autoRefuelPercent=50,
				fuelCapacity=15.0f,
				showFuelGizmo=false,
				fuelFilter=new ThingFilter()
			};
			this.secondRefuelComp.props=compProperties_Refuelable;
			this.secondRefuelComp.parent=this.firstRefuelComp.parent;
			this.secondRefuelComp.Props.fuelLabel="Bag 2";
			this.firstRefuelComp.Props.fuelLabel="Bag 1";
		}

		public override void ExposeData() {
			Scribe_Values.Look<float>(ref this.secondFuelCount, "secondFuelCount", 0f);
			Scribe_Defs.Look<ThingDef>(ref this.firstFuelType, "firstFuelType");
			Scribe_Defs.Look<ThingDef>(ref this.secondFuelType, "secondFuelType");
		}

		public override void Tick() {
			if(Find.TickManager.TicksGame%60==0) // for performance
			{
				base.Tick();
				if(this.firstTime==true) {
					this.secondRefuelComp.Refuel(this.secondFuelCount);
					this.firstTime=false;
				}
				if(this.secondFuelCount!=secondRefuelComp.Fuel) {
					this.secondFuelCount=secondRefuelComp.Fuel;
				}
				if(this.oldFirstFuelType!=this.firstFuelType) {
					string oldTexPath = oldFirstFuelType.defName;
					string newTexPath = firstFuelType.defName;
					Log.Message(oldTexPath);
					Log.Message(newTexPath);
					string texPath = "";
					//Graphic value = GraphicDatabase.Get(this.def.graphicData.graphicClass, texPath, 
					//    ShaderDatabase.LoadShader(this.def.graphicData.shaderType.shaderPath), 
					//    this.def.graphicData.drawSize, this.DrawColor, this.DrawColorTwo);
					//typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, value);
					//if (this.Map != null)
					//{
					//    this.DirtyMapMesh(this.Map);
					//}
				}
				bool flag2 = this.firstRefuelComp.HasFuel&&this.flickableComp.SwitchIsOn;
				if(flag2) {
					bool flag3 = this.ActivePawns.ToList<Pawn>().Count>0;
					if(flag3) {
						this.ManageActivePawns();
					}
					this.ApplyIV();
				}
				else {
					this.ActivePawns.Clear();
				}

				bool sflag2 = this.secondRefuelComp.HasFuel&&this.flickableComp.SwitchIsOn;
				if(sflag2) {
					bool sflag3 = this.ActivePawns.ToList<Pawn>().Count>0;
					if(sflag3) {
						this.ManageActivePawns();
					}
					this.ApplyIV();
				}
				else {
					this.ActivePawns.Clear();
				}

				this.oldFirstFuelType=this.firstFuelType;
				this.oldSecondFuelType=this.secondFuelType;
			}
		}

		public override string GetInspectString() {
			StringBuilder stringBuilder = new StringBuilder();
			try {
				if(this.firstFuelType!=null) {
					stringBuilder.Append("firstFuel".Translate()+": "+this.firstFuelType.label+"\n");
					stringBuilder.Append("firstFuelCount".Translate()+": "+this.firstRefuelComp.Fuel.ToString()+"\n");
				}
			}
			catch { }

			try {
				if(this.secondFuelType!=null) {
					stringBuilder.Append("secondFuel".Translate()+": "+this.secondFuelType.label+"\n");
					stringBuilder.Append("secondFuelCount".Translate()+": "+this.secondRefuelComp.Fuel.ToString()+"\n");
				}
			}
			catch { }
			stringBuilder.Append(base.GetInspectString());

			return stringBuilder.ToString();
		}

		public void ApplyIV() {
			IntVec3[] cardinalDirectionsAround = GenAdj.CardinalDirectionsAround;
			IntVec3 position = base.Position;
			for(int i = 0; i<cardinalDirectionsAround.Length; i++) {
				List<Thing> list = base.Map.thingGrid.ThingsListAt(cardinalDirectionsAround[i]+position);
				foreach(Thing thing in list) {
					bool flag = thing is Pawn;
					if(flag) {
						Pawn pawn = thing as Pawn;
						bool flag2 = this.ActivePawns.Contains(pawn);
						if(!flag2) {
							bool flag3 = (pawn.RaceProps.Humanlike&&pawn.InBed())||(pawn.RaceProps.Animal&&pawn.InBed());
							if(flag3) {
								this.ActivePawns.Add(pawn);
								this.ManageActivePawns();
							}
						}
					}
				}
			}
		}

		public void ManageActivePawns() {
			foreach(Pawn pawn in this.ActivePawns.ToList<Pawn>()) {
				this.firstRefuelComp.ConsumeFuel(0.0075f);
				bool flag = pawn.InBed();
				if(flag) {
					pawn.health.AddHediff(IV_Stand.IV_BloodTransfusion, null, null, null);
				}
				else {
					this.ActivePawns.Remove(pawn);
				}
			}
		}

		public static HediffDef IV_BloodTransfusion = HediffDef.Named("BadBack");
		public CompRefuelable firstRefuelComp = null;
		public ThingDef firstFuelType = null;
		public ThingDef oldFirstFuelType = null;
		public CompRefuelable secondRefuelComp = null;
		public ThingDef secondFuelType = null;
		public ThingDef oldSecondFuelType = null;
		public float secondFuelCount = 0f;
		private CompFlickable flickableComp = null;
		private bool firstTime = true;
		private List<Pawn> ActivePawns = new List<Pawn>();
	}
}
