using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MedicalOverhaul
{
	public static class Utils
	{
		public static string IngredientFilterSummary(ThingFilter thingFilter)
		{
			return thingFilter.Summary;
		}

		public static string VowelTrim(string str, int limit)
		{
			int num = str.Length - limit;
			int num2 = str.Length - 1;
			while (num2 > 0 && num > 0)
			{
				if (Utils.IsVowel(str[num2]) && str[num2 - 1] != ' ')
				{
					str = str.Remove(num2, 1);
					num--;
				}
				num2--;
			}
			if (str.Length > limit)
			{
				str = str.Remove(limit - 2) + "..";
			}
			return str;
		}

		public static bool IsVowel(char c)
		{
			return new HashSet<char>
			{
				'a',
				'e',
				'i',
				'o',
				'u'
			}.Contains(c);
		}

		public static Texture2D GetIcon(ThingDef thingDef)
		{
			Texture2D texture2D = ContentFinder<Texture2D>.Get(thingDef.graphicData.texPath, false);
			if (texture2D == null)
			{
				texture2D = ContentFinder<Texture2D>.GetAllInFolder(thingDef.graphicData.texPath).ToList<Texture2D>()[0];
				if (texture2D == null)
				{
					texture2D = ContentFinder<Texture2D>.Get("UI/Commands/LaunchReport", true);
					Log.Warning("Universal Fermenter:: No texture at " + thingDef.graphicData.texPath + ".", false);
				}
			}
			return texture2D;
		}
	}
}
