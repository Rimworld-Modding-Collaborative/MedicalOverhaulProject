using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompExtraRecipeProducts
{

    public class CompProperties_ExtraRecipeProducts : CompProperties
    {
        public CompProperties_ExtraRecipeProducts()
        {
            this.compClass = typeof(CompExtraRecipeProducts);
        }
        public List<ExtraProducts> ExtraProducts = new List<ExtraProducts>();
        public bool pickrandom = false;
        public int take = -1;
    }

    public class ExtraProducts
    {
        public ThingDef productdef = null;
        public int Min = 1;
        public int Max = 1;
    }

    public class CompExtraRecipeProducts : ThingComp
    {
        public CompProperties_ExtraRecipeProducts Props=> (CompProperties_ExtraRecipeProducts)this.props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            parent.Destroy();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            if (Props.pickrandom)
            {
                if (Props.take > 0)
                {
                    for (int i = 0; i < Props.take; i++)
                    {
                        ExtraProducts set = Props.ExtraProducts.RandomElement();
                        Thing thing = ThingMaker.MakeThing(set.productdef, null);
                        thing.stackCount = Rand.RangeInclusive(set.Min, set.Max);
                        if (thing.stackCount > 0)
                        {
                            GenPlace.TryPlaceThing(thing, base.parent.Position, map, ThingPlaceMode.Near, null, null);
                        }
                    }
                }
                else
                {
                    ExtraProducts set = Props.ExtraProducts.RandomElement();
                    Thing thing = ThingMaker.MakeThing(set.productdef, null);
                    thing.stackCount = Rand.RangeInclusive(set.Min, set.Max);
                    if (thing.stackCount > 0)
                    {
                        GenPlace.TryPlaceThing(thing, base.parent.Position, map, ThingPlaceMode.Near, null, null);
                    }
                }
            }
            else
                foreach (ExtraProducts set in Props.ExtraProducts)
                {
                    Thing thing = ThingMaker.MakeThing(set.productdef, null);
                    thing.stackCount = Rand.RangeInclusive(set.Min, set.Max);
                    if (thing.stackCount > 0)
                    {
                        GenPlace.TryPlaceThing(thing, base.parent.Position, map, ThingPlaceMode.Near, null, null);
                    }
                }
        }
        
    }
 
}
