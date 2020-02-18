using System;
using Verse;

namespace MedicalOverhaul
{
    public class PawnData : IExposable
    {
        public PawnData()
        {
        }

        public PawnData(int totalChronicDiseases, int daysCounter)
        {
            this.totalChronicDiseases = totalChronicDiseases;
            this.daysCounter = daysCounter;
        }
    public void ExposeData()
        {
            Scribe_Values.Look<int>(ref this.totalChronicDiseases, "totalChronicDiseases", 0, true);
            Scribe_Values.Look<int>(ref this.daysCounter, "daysCounter", 0, true);
        }

        public int totalChronicDiseases;
        public int daysCounter;

    }
}
