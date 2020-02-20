using System;

using System.IO;
using System.Collections.Generic;
using RimWorld.Planet;
using Verse;
using RimWorld;

namespace MedicalOverhaul
{
    public class BagsToBeLoaded : MapComponent
    {
        public BagsToBeLoaded(Map map) : base(map)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<BagData>(ref this.bagsToBeLoaded, "bagsToBeLoaded", LookMode.Reference);
        }

        public List<BagData> bagsToBeLoaded = new List<BagData>();
    }
}
