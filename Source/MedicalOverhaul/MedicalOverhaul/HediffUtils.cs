using System;
using System.Linq;
using RimWorld;
using Verse;

namespace MedicalOverhaul
{
    public static class HediffUtils
    {
        public static float getDeathTimeInHours(Hediff hediff)
        {
            HediffComp_Immunizable hediffComp_Immunizable = hediff.TryGetComp<HediffComp_Immunizable>();
            if (hediffComp_Immunizable != null)
                return 24f * (1.0f - hediff.Severity) / hediffComp_Immunizable.Props.severityPerDayNotImmune;

            return 0f;
        }

        public static void setDeathTime(Hediff hediff, int minHour, int maxHour)
        {
            HediffComp_Immunizable hediffComp_Immunizable = hediff.TryGetComp<HediffComp_Immunizable>();
            if (hediffComp_Immunizable != null)
            {
                Random random = new Random();
                float max = (1.0f - hediff.Severity) / ((((float)maxHour / 24) * 100) / 100);
                float min = (1.0f - hediff.Severity) / ((((float)minHour / 24) * 100) / 100);
                var next = random.NextDouble();
                float deathTime = (float)(min + (next * (max - min))); // death time between min and max hours
                try
                {
                    hediffComp_Immunizable.Props.severityPerDayNotImmune = deathTime;
                }
                catch (Exception ex)
                {
                    Log.Message(ex.ToString());
                    Log.Message(ex.StackTrace);
                }
            }
        }
        public static void GiveHediffToPawn(Pawn pawn, HediffDef hediffDef, BodyPartDef partDef = null, int? minHour = null, int? maxHour = null)
        {
            try
            {
                BodyPartRecord part = null;
                if (partDef != null)
                    part = pawn.def.race.body.AllParts.FirstOrDefault((BodyPartRecord x) => x.def == partDef);
                Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn, part);
                Log.Message(pawn.Label + " receives hediff " + hediff.Label);
                if (minHour.HasValue && maxHour.HasValue)
                {
                    setDeathTime(hediff, minHour.Value, maxHour.Value);
                    Log.Message(pawn.Label + " will die in (randomized) " +  getDeathTimeInHours(hediff).ToString() + " hours");
                }
                else
                {
                    Log.Message(pawn.Label + " will die in (not randomized) " +  getDeathTimeInHours(hediff).ToString() + " hours");
                }
                pawn.health.AddHediff(hediff);
            }
            catch (Exception ex)
            {
                Log.Message(ex.ToString());
                Log.Message(ex.StackTrace);
            }

        }
    }
}
