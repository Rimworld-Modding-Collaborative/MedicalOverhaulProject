using System;
using Verse;

namespace MedicalOverhaul
{
    public class PawnData : IExposable
    {
        public PawnData()
        {
        }

        public PawnData(int totalChronicDiseases)
        {
            this.totalChronicDiseases = totalChronicDiseases;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref this.totalChronicDiseases, "totalChronicDiseases", 0, true);
        }

        public int totalChronicDiseases;

    }
}
