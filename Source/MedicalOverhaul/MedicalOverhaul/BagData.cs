using System;
using RimWorld;
using Verse;

namespace MedicalOverhaul
{
    public class BagData : IExposable
    {
        public BagData()
        {
        }

        public BagData(IV_Stand stand, ThingDef bagDef, string fuelType)
        {
            this.stand = stand;
            this.bagDef = bagDef;
            this.fuelType = fuelType;
        }
    public void ExposeData()
        {
            Scribe_Values.Look<IV_Stand>(ref this.stand, "stand", null, true);
            Scribe_Values.Look<ThingDef>(ref this.bagDef, "bagDef", null, true);
            Scribe_Values.Look<string>(ref this.fuelType, "fuelType", null, true);
        }

        public IV_Stand stand;
        public ThingDef bagDef;
        public string fuelType;

    }
}
