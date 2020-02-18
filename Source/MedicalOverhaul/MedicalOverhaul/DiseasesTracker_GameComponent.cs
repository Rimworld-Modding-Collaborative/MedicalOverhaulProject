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
        public int maxDiseases = 2;
        public int diseaseTimer = 0;
        public int randomChance = 2;
        public int totalDays = 2;
        public int gracePediod = 5;
        public List<int> diseasesDays;
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


        public void GiveRandomHediff(Pawn pawn)
        {
            Random random = new Random(pawn.GetUniqueLoadID().GetHashCode() + Find.TickManager.TicksGame);
            int index = random.Next(this.chronicDiseases.Count);
            HediffUtils.GiveHediffToPawn(pawn, this.chronicDiseases[index]);
            Find.LetterStack.ReceiveLetter("Chronic disease", pawn.Label + " receives chronic disease - " + this.chronicDiseases[index].defName, LetterDefOf.NegativeEvent, null);
            Log.Message(pawn.Label + " receives chronic disease - " + this.chronicDiseases[index].defName);
        }


        public void CheckDiseaseTracker()
        {
            Log.Message("CheckDiseaseTracker");
            List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners;
            Random random = new Random();
            var shuffledPawns = pawns.OrderBy(item => random.Next());
            foreach (Pawn pawn in shuffledPawns)
            {
                PawnData data = new PawnData();
                bool exists = this.PawnsData.TryGetValue(pawn, out data);
                if (exists != true)
                {
                    Log.Message(pawn.Label + " missing in PawnsData, adding...");
                    data.totalChronicDiseases = 0;
                    this.PawnsData.Add(pawn, data);
                }
                if (data.totalChronicDiseases <= maxDiseases)
                {
                    GiveRandomHediff(pawn);
                    data.totalChronicDiseases += 1;
                    break;
                }
            }
        }

        public void ResetDiseasesDays()
        {
            Log.Message("ResetDiseasesDays");
            this.diseasesDays = new List<int>();
            for (var i = 0; i < this.totalDays; i++)
            {
                while (true)
                {
                    Random random = new Random();
                    int nextDay = random.Next(GenDate.DaysPassed, GenDate.DaysPassed + 60);
                    if (!diseasesDays.Contains(nextDay) && nextDay >= this.gracePediod)
                    {
                        Log.Message("nextDay: " + nextDay.ToString());
                        diseasesDays.Add(nextDay);
                        break;
                    }
                }
            }
        }
        public override void StartedNewGame()
        {
            base.StartedNewGame();
            this.PawnsData = new Dictionary<Pawn, PawnData>();
            ResetDiseasesDays();
        }
         
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (Find.TickManager.TicksGame % 60 == 0) // for performance
            {
                if (this.diseasesDays == null)
                {
                    ResetDiseasesDays();
                }

                for (int i = this.diseasesDays.Count - 1; i >= 0; i--)
                {
                    if (GenDate.DaysPassed > this.diseasesDays[i])
                    {
                        Log.Message("GenDate.DaysPassed > this.diseasesDays[i]");
                        this.diseasesDays.RemoveAt(i);
                    }
                }

                if (!this.diseasesDays.Any())
                {
                    Log.Message("if (!this.diseasesDays.Any())");
                    ResetDiseasesDays();
                }


                int? dayToRemove = null;
                foreach (int day in this.diseasesDays)
                {
                    if (GenDate.DaysPassed == day)
                    {
                        Log.Message("GenDate.DaysPassed == day");
                        int nextTime = rnd.Next(0, 120000);
                        Log.Message("this.diseaseTimer = Find.TickManager.TicksGame + nextTime;");
                        this.diseaseTimer = Find.TickManager.TicksGame + nextTime;
                        dayToRemove = day;
                    }
                }
                if (dayToRemove.HasValue)
                {
                    Log.Message("Remove day " + dayToRemove.Value.ToString());
                    this.diseasesDays.Remove(dayToRemove.Value);
                }
                    
                if (this.diseaseTimer != 0 && Find.TickManager.TicksGame > this.diseaseTimer)
                {
                    Log.Message("this.diseaseTimer: " + this.diseaseTimer.ToString());
                    this.diseaseTimer = 0;
                    Log.Message("Find.TickManager.TicksGame > this.diseaseTimer");
                    CheckDiseaseTracker();
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look<Pawn, PawnData>(ref this.PawnsData, "PawnData", LookMode.Reference, LookMode.Deep,
                ref this.pawnKeysWorkingList, ref this.pawnDataValuesWorkingList);
            Scribe_Values.Look<int>(ref this.diseaseTimer, "DiseaseTimer", 0, true);
            Scribe_Collections.Look<int>(ref this.diseasesDays, "DiseasesDays", LookMode.Undefined, new object[0]);
            Log.Message("this.diseaseTimer: " + this.diseaseTimer.ToString());
        }

        private List<Pawn> pawnKeysWorkingList;
        private List<PawnData> pawnDataValuesWorkingList;
    }
}