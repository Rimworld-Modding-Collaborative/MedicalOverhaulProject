using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MedicalOverhaul
{
    public class DiseasesTracker : GameComponent
    {
        public DiseasesTracker()
        {

        }
        public List<HediffDef> totalHediffes;
        public List<HediffDef> chronicDiseases;
        public Dictionary<Pawn, PawnData> PawnsData;
        public int max = 2;
        public int diseaseTimer;
        public int randomChance = 2;
        public Random rnd = new Random();

        public DiseasesTracker(Game game)
        {
            totalHediffes = DefDatabase<HediffDef>.AllDefsListForReading;
            List<string> chronicDiseasesStrings = new List<string>() {
                "BadBack",
                "Frail",
                "Cataract",
                "Blindness",
                "HearingLoss",
                "Dementia",
                "Alzheimers",
                "Asthma",
                "HeartArteryBlockage",
                "Carcinoma"
            };
            chronicDiseases = totalHediffes.Where(x => chronicDiseasesStrings.Any(y => y == x.defName)).ToList();
        }


        public bool TryGiveHediffRandom(Pawn pawn, float randomChance)
        {
            Random random = new Random();
            if (random.Next(0, 100) < randomChance)
            {
                int index = random.Next(this.chronicDiseases.Count);
                HediffUtils.GiveHediffToPawn(pawn, this.chronicDiseases[index]);
                Find.LetterStack.ReceiveLetter("Chronic disease", pawn.Label + " receives chronic disease - " + this.chronicDiseases[index].defName, LetterDefOf.NegativeEvent, null);
                Log.Message(pawn.Label + " receives chronic disease - " + this.chronicDiseases[index].defName);
                return true;
            }
            return false;
        }
        public void CheckDiseaseTracker()
        {
            Log.Message("CheckDiseaseTracker");
            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
            {
                PawnData data = new PawnData();
                bool exists = this.PawnsData.TryGetValue(pawn, out data);
                if (data.daysCounter > 60)
                    data.totalChronicDiseases = 0;
                if (exists == true)
                {
                    if (data.totalChronicDiseases < 3)
                    {
                        bool givenHediff = TryGiveHediffRandom(pawn, this.randomChance);
                        if (givenHediff == true)
                        {
                            data.totalChronicDiseases += 1;
                        }
                    }
                }
                else
                {
                    Log.Message(pawn.Label + " missing in PawnsData, adding...");
                    PawnData values = new PawnData();
                    values.totalChronicDiseases = 0;
                    this.PawnsData.Add(pawn, values);
                }
                data.daysCounter += 1;
            }
        }
        public override void StartedNewGame()
        {
            base.StartedNewGame();
            this.PawnsData = new Dictionary<Pawn, PawnData>();
            this.diseaseTimer = Find.TickManager.TicksGame + rnd.Next(0, 120000);
            CheckDiseaseTracker();
        }
         
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (Find.TickManager.TicksGame > this.diseaseTimer)
            {
                CheckDiseaseTracker();
                this.diseaseTimer = Find.TickManager.TicksGame + rnd.Next(0, 120000); // between 0 and 2 days

            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look<Pawn, PawnData>(ref this.PawnsData, "PawnData", LookMode.Reference, LookMode.Deep,
                ref this.pawnKeysWorkingList, ref this.pawnDataValuesWorkingList);
            Scribe_Values.Look<int>(ref this.diseaseTimer, "DiseaseTimer", Find.TickManager.TicksGame + rnd.Next(0, 120000), true);


        }

        private List<Pawn> pawnKeysWorkingList;
        private List<PawnData> pawnDataValuesWorkingList;
    }
}