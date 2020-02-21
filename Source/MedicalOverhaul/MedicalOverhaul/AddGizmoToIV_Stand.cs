using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;

namespace MedicalOverhaul
{
    [HarmonyPatch(typeof(Building), "GetGizmos")]
    public class Patch_Building_GetGizmos
    {
        private static void Postfix(Building __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance.def.defName == "MOP_IV_BaseDrip")
            {
                List<ThingDef> fuelTypes = new List<ThingDef>() { };
                List<string> fuelNames = new List<string>()
                {
                    "MOP_BloodBag",
                    "MOP_TestBag"
                };
                foreach (string defName in fuelNames)
                {
                    fuelTypes.Add(DefDatabase<ThingDef>.GetNamed(defName, true));
                }
                var building = (IV_Stand)__instance;                
                List <Gizmo> list = new List<Gizmo>(__result)
                {
                    new Command_Action
                    {
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/LoadTransporter", true),
                        defaultLabel = "MOP_FirstFuelLabel".Translate(),
                        defaultDesc = "MOD_FirstFuelDescr".Translate(),
                        activateSound = SoundDef.Named("Click"),
                        action = delegate()
                        {
                            Find.WindowStack.Add(MakeFuelMenu(__instance, fuelTypes, "first"));
                        },
                        groupKey = 927767552
                    },
                    new Command_Action
                    {
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/LoadTransporter", true),
                        defaultLabel = "MOP_SecondFuelLabel".Translate(),
                        defaultDesc = "MOD_SecondFuelDescr".Translate(),
                        activateSound = SoundDef.Named("Click"),
                        action = delegate()
                        {
                            Find.WindowStack.Add(MakeFuelMenu(__instance, fuelTypes, "second"));
                        },
                        groupKey = 927767553
                    },
                };
                __result = list;
            }
        }

        public static FloatMenu MakeFuelMenu(Building __instance, List<ThingDef> fuelTypes, string fuelType)
        {
            List<FloatMenuOption> floatMenu = new List<FloatMenuOption>();
            foreach (ThingDef thingDef in fuelTypes)
            {
                floatMenu.Add(new FloatMenuOption(thingDef.LabelCap, delegate ()
                {
                    Log.Message(__instance.Label + " to " + thingDef.defName + " in " + fuelType + " fuel type");
                    BagData bagData = new BagData();
                    bagData.bagDef = thingDef;
                    bagData.fuelType = fuelType;
                    bagData.stand = (IV_Stand)__instance;
                    __instance.Map.GetComponent<BagsToBeLoaded>().bagsToBeLoaded.Add(bagData);
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }

            return new FloatMenu(floatMenu);
        }
    }
}
