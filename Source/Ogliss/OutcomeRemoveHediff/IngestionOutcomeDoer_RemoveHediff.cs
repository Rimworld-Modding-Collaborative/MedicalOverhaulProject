using System;
using System.Collections.Generic;
using UnityEngine;
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
            //    Log.Message(string.Format("{0} {1}, minSeverity: ({2}), initialSeverity: ({3}), maxSeverity: ({4}), leathalSeverity: ({5}), Reduction factor: ({6} - {7})", pawn.LabelShortCap, hediffDef.LabelCap, hediffDef.minSeverity, hediffDef.initialSeverity, hediffDef.maxSeverity, hediffDef.lethalSeverity, range.min, range.max));
                Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.hediffDef);
                float factor = range.RandomInRange;
                float num = (hediff.Severity <= hediffDef.initialSeverity ? hediffDef.initialSeverity : hediffDef.maxSeverity) * factor;
                Mathf.Clamp(num, hediffDef.minSeverity, hediffDef.maxSeverity);
            //    Log.Message(string.Format("reducing {1} severity by {2}", pawn.LabelShortCap, hediff.LabelCap, num));
                hediff.Severity = hediff.Severity - num;
            //    Log.Message(string.Format("Reduced to {0}", hediff.Severity));
                if (hediff.Severity <= Math.Max(0f, hediffDef.minSeverity))
                {
                //    Log.Message(string.Format("Reduced below {0}'s minSeverity ({1}) Removing", hediff.LabelCap, hediffDef.minSeverity));
                    pawn.health.RemoveHediff(hediff);
                }
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

        public FloatRange range = FloatRange.ZeroToOne;
    }
}
