using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace MedicalOverhaul {
	public class Building_DripStand : Building {
		public Building_DripStand() { }

		public override void SpawnSetup(Map map, bool respawningAfterLoad) {
			base.SpawnSetup(map, respawningAfterLoad);
			this.refuelComp=base.GetComp<Comp_TwoBagIV>();
		}
		public override void Tick() {
			if(Find.TickManager.TicksGame%60==0) { // for performance
				base.Tick();
				bool noFuel_orOff = (this.refuelComp.Bag1_HasFuel||this.refuelComp.Bag2_HasFuel)&&this.flickableComp.SwitchIsOn;
				if(noFuel_orOff) {
					bool pawnsActive = this.ActivePawns.ToList<Pawn>().Count>0;
					if(pawnsActive) {
						this.ManageActivePawns();
					}
					this.ApplyIV();
				}
				else {
					this.ActivePawns.Clear();
				}
			}
		}

		public override string GetInspectString() {
			StringBuilder stringBuilder = new StringBuilder();
			try {
				if(this.refuelComp!=null) {
					stringBuilder.Append("dripStands_firstBag".Translate()+": "+this.refuelComp.Props.Bag1_Label+"\n");
					stringBuilder.Append("dripStands_firstBagCount".Translate()+": ("+this.refuelComp.Bag1+"/"+this.refuelComp.Props.bag1_Capacity+")\n");

					stringBuilder.Append("dripStands_secondBag".Translate()+": "+this.refuelComp.Props.Bag2_Label+"\n");
					stringBuilder.Append("dripStands_secondBagCount".Translate()+": ("+this.refuelComp.Bag2+"/"+this.refuelComp.Props.bag2_Capacity+")");
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
							bool flag3 = (pawn.RaceProps.Humanlike||pawn.RaceProps.Animal)&&pawn.InBed();
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
				this.refuelComp.ConsumeFuel(0.0075f);
				bool flag = pawn.InBed();
				if(flag) {
					pawn.health.AddHediff(IV_Stand.IV_BloodTransfusion, null, null, null);
				}
				else {
					this.ActivePawns.Remove(pawn);
				}
			}
		}

		public Comp_TwoBagIV refuelComp = null;
		private CompFlickable flickableComp = null;
		private List<Pawn> ActivePawns = new List<Pawn>();
		private ThingDef bag1;
		private ThingDef bag2;
	}
}
