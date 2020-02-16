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
            {
                return 24f * (1.0f - hediff.Severity) / hediffComp_Immunizable.Props.severityPerDayNotImmune;
            }
            return 0f;
        }

        public static void setDeathTime(Hediff hediff, int? minHour = null, int? maxHour = null)
        {
            if (minHour.HasValue && maxHour.HasValue)
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
        }
        public static void GiveHediffToPawn(Pawn pawn, HediffDef hediffDef, string partName = null, int? minHour = null, int? maxHour = null)
        {
            BodyPartRecord part = null;
            if (partName != null)
            {
                part = pawn.def.race.body.AllParts.FirstOrDefault((BodyPartRecord x) => x.def.defName == partName);
            }
            Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn, part);
            setDeathTime(hediff, minHour, maxHour);
            pawn.health.AddHediff(hediff);
            Log.Message(pawn.Label + " receives hediff " + hediff.Label);
        }
    }
}
