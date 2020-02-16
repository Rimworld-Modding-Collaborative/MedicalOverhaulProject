using System;
using System.Collections.Generic;
using Verse;

namespace MedicalOverhaul
{
	public class CompProperties_UniversalFermenter : CompProperties
	{
		public CompProperties_UniversalFermenter()
		{
			this.compClass = typeof(CompUniversalFermenter);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			base.ResolveReferences(parentDef);
			for (int i = 0; i < this.products.Count; i++)
			{
				this.products[i].ResolveReferences();
			}
		}

		public List<UniversalFermenterProduct> products = new List<UniversalFermenterProduct>();
	}
}
