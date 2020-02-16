using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x02000275 RID: 629
    public class IngestionOutcomeDoer_RemoveHediff : IngestionOutcomeDoer
    {
        // Token: 0x06000AED RID: 2797 RVA: 0x00057114 File Offset: 0x00055514
        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if (pawn.health.hediffSet.HasHediff(this.hediffDef))
            {
                Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.hediffDef);
                pawn.health.RemoveHediff(hediff);
            }
        }

        // Token: 0x06000AEE RID: 2798 RVA: 0x00057198 File Offset: 0x00055598
        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            if (parentDef.IsDrug && this.chance >= 1f)
            {
                foreach (StatDrawEntry s in this.hediffDef.SpecialDisplayStats(StatRequest.ForEmpty()))
                {
                    yield return s;
                }
            }
            yield break;
        }

        // Token: 0x0400053A RID: 1338
        public HediffDef hediffDef;
    }
}
