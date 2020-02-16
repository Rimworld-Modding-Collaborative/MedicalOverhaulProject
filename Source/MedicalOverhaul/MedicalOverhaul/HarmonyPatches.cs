using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
                            if (!pawn.health.hediffSet.hediffs.Exists((Hediff x) => x.def == HediffDefOf.RespiratoryFailure))
                            {
                                Random random = new Random();
                                if (random.Next(0, 100) < 20)
                                {
                                    HediffUtils.GiveHediffToPawn(pawn, HediffDefOf.RespiratoryFailure, "Torso", 7, 16);
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
                                    HediffUtils.GiveHediffToPawn(pawn, HediffDefOf.RespiratoryFailure, BodyPartDefOf.Neck.defName, 3, 9);
                                }
                            }
                        }
                    }
                }
            }
        }
        public static FieldInfo pawn = typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }


    [HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDeadFromRequiredCapacity")]
    public static class ShouldBeDeadFromRequiredCapacityPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> MedicalOverhaulException(IEnumerable<CodeInstruction> instrs, ILGenerator gen)
        {
            bool trigger = false;
            foreach (CodeInstruction itr in instrs)
            {
                yield return itr;
                if (trigger)
                {
                    trigger = false;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_HealthTracker), "pawn"));
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ShouldBeDeadFromRequiredCapacityPatch), "AddCustomHediffs", new Type[] { typeof(Pawn_HealthTracker), typeof(Pawn), typeof(PawnCapacityDef) }));
                    yield return itr;
                }
                if (itr.opcode == OpCodes.Callvirt && itr.operand == AccessTools.Method(typeof(PawnCapacitiesHandler), "CapableOf", new Type[] { typeof(PawnCapacityDef) }))
                {
                    trigger = true;
                }
            }
        }

        public static bool AddCustomHediffs(Pawn_HealthTracker tracker, Pawn pawn, PawnCapacityDef pawnCapacityDef)
        {
            bool result = false;
            if (pawn.RaceProps.IsFlesh && pawnCapacityDef.lethalFlesh && !tracker.capacities.CapableOf(pawnCapacityDef))
            {
                if (pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault(p => p.def.defName == "Lung") == null)
                {
                    if (!pawn.health.hediffSet.HasHediff(HediffDefOf.RespiratoryFailure))
                    {
                        HediffUtils.GiveHediffToPawn(pawn, HediffDefOf.RespiratoryFailure, null, 3, 9);
                    }
                    else
                    {
                        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RespiratoryFailure);
                        if (HediffUtils.getDeathTimeInHours(hediff) > 9f)
                        {
                            HediffUtils.GiveHediffToPawn(pawn, HediffDefOf.RespiratoryFailure, null, 3, 9);
                        }
                    }
                    result = true;
                }

                if (pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault(p => p.def.defName == "Kidney") == null)
                {
                    if (!pawn.health.hediffSet.HasHediff(HediffDefOf.RenalFailure))
                    {
                        HediffUtils.GiveHediffToPawn(pawn, HediffDefOf.RenalFailure, null);
                    }
                    //else
                    //{
                    //    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RespiratoryFailure);
                    //    if (HediffUtils.getDeathTimeInHours(hediff) > 9f)
                    //    {
                    //        HediffUtils.GiveHediffToPawn(pawn, HediffDefOf.IntestinalFailure, null);
                    //    }
                    //}
                    result = true;
                }

                if (pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault(p => p.def.defName == "Liver") == null)
                {
                    if (!pawn.health.hediffSet.HasHediff(HediffDefOf.LiverFailure))
                    {
                        HediffUtils.GiveHediffToPawn(pawn, HediffDefOf.LiverFailure, null);
                    }
                    //else
                    //{
                    //    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RespiratoryFailure);
                    //    if (HediffUtils.getDeathTimeInHours(hediff) > 9f)
                    //    {
                    //        HediffUtils.GiveHediffToPawn(pawn, HediffDefOf.IntestinalFailure, null);
                    //    }
                    //}
                    result = true;
                }

                if (pawn.health.hediffSet.GetNotMissingParts().FirstOrDefault(p => p.def.defName == "Stomach") == null)
                {
                    if (!pawn.health.hediffSet.HasHediff(HediffDefOf.IntestinalFailure))
                    {
                        HediffUtils.GiveHediffToPawn(pawn, HediffDefOf.IntestinalFailure, null);
                    }
                    //else
                    //{
                    //    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RespiratoryFailure);
                    //    if (HediffUtils.getDeathTimeInHours(hediff) > 9f)
                    //    {
                    //        HediffUtils.GiveHediffToPawn(pawn, HediffDefOf.IntestinalFailure, null);
                    //    }
                    //}
                    result = true;
                }
            }
            return result;
        }
    }
}
