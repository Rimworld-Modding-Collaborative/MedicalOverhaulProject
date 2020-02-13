using System;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace MedicalOverhaul
{
    [StaticConstructorOnStartup]
    internal static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("rimworld.MedicalOverhaul.org");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange", null)]
    public static class CheckForStateChange_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff)
        {
            if (dinfo.HasValue)
            {
                DamageInfo dinfo2 = dinfo.Value;
                if (dinfo2.HitPart != null)
                {
                    if (dinfo2.HitPart.def != null)
                    {
                        Pawn pawn = (Pawn)CheckForStateChange_Patch.pawn.GetValue(__instance);
                        if (dinfo2.HitPart.def.defName == "Lung")
                        {
                            bool noLungs = true;
                            foreach (var organ in pawn.health.hediffSet.GetNotMissingParts())
                            {
                                if (organ.def.defName == "Lung")
                                {
                                    noLungs = false;
                                    break;
                                }
                            }
                            if (noLungs == true)
                            {
                                if (!pawn.health.hediffSet.hediffs.Exists((Hediff x) => x.def == HediffDefOf.RespiratoryFailure))
                                {
                                    Hediff RespiratoryFailureHediff = HediffMaker.MakeHediff(HediffDefOf.RespiratoryFailure, pawn,  null);
                                    pawn.health.AddHediff(RespiratoryFailureHediff);
                                    Log.Message(pawn.Label + " receives hediff " + RespiratoryFailureHediff.Label);
                                }
                            }
                        }
                        else if (dinfo2.HitPart.def.defName == BodyPartDefOf.Neck.defName)
                        {
                            if (!pawn.health.hediffSet.hediffs.Exists((Hediff x) => x.def == HediffDefOf.RespiratoryFailure))
                            {
                                Random random = new Random();
                                if (random.Next(0, 100) < 30)
                                {

                                    Hediff RespiratoryFailureHediff = HediffMaker.MakeHediff(HediffDefOf.RespiratoryFailure, pawn, null);
                                    pawn.health.AddHediff(RespiratoryFailureHediff);
                                    Log.Message(pawn.Label + " receives hediff " + RespiratoryFailureHediff.Label);
                                }
                            }
                        }
                    }
                }
            }
        }
        public static FieldInfo pawn = typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }
}
