using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MedicalOverhaul
{
	public class CompUniversalFermenter : ThingComp
	{
		public CompProperties_UniversalFermenter Props
		{
			get
			{
				return (CompProperties_UniversalFermenter)this.props;
			}
		}

		private int ResourceListSize
		{
			get
			{
				return this.Props.products.Count;
			}
		}

		public UniversalFermenterProduct Product
		{
			get
			{
				return this.Props.products[this.currentResourceInd];
			}
		}

		public UniversalFermenterProduct NextProduct
		{
			get
			{
				return this.Props.products[this.nextResourceInd];
			}
		}

		public bool Ruined
		{
			get
			{
				return this.ruinedPercent >= 1f;
			}
		}

		public string SummaryAddedIngredients
		{
			get
			{
				int num = 60;
				string text = "";
				for (int i = 0; i < this.ingredientLabels.Count; i++)
				{
					if (i == 0)
					{
						text += this.ingredientLabels[i];
					}
					else
					{
						text = text + ", " + this.ingredientLabels[i];
					}
				}
				int length = string.Concat(new string[]
				{
					"Contains ",
					this.Product.maxCapacity.ToString(),
					"/",
					this.Product.maxCapacity.ToString(),
					" "
				}).Length;
				int limit = num - length;
				return Utils.VowelTrim(text, limit);
			}
		}

		public string SummaryNextIngredientFilter
		{
			get
			{
				return Utils.IngredientFilterSummary(this.NextProduct.ingredientFilter);
			}
		}

		// (set) Token: 0x0600000B RID: 11 RVA: 0x000020EE File Offset: 0x000002EE
		public float Progress
		{
			get
			{
				return this.progressInt;
			}
			set
			{
				if (value == this.progressInt)
				{
					return;
				}
				this.progressInt = value;
				this.barFilledCachedMat = null;
			}
		}

		private Material BarFilledMat
		{
			get
			{
				if (this.barFilledCachedMat == null)
				{
					this.barFilledCachedMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.Lerp(Static_Bar.ZeroProgressColor, Static_Bar.FermentedColor, this.Progress), false);
				}
				return this.barFilledCachedMat;
			}
		}

		private bool Empty
		{
			get
			{
				return this.ingredientCount <= 0;
			}
		}

		public bool Fermented
		{
			get
			{
				return !this.Empty && this.Progress >= 1f;
			}
		}

		public int SpaceLeftForIngredient
		{
			get
			{
				if (this.Fermented)
				{
					return 0;
				}
				return this.Product.maxCapacity - this.ingredientCount;
			}
		}

		private void NextResource()
		{
			this.nextResourceInd++;
			if (this.nextResourceInd >= this.ResourceListSize)
			{
				this.nextResourceInd = 0;
			}
			if (this.Empty)
			{
				this.currentResourceInd = this.nextResourceInd;
			}
		}

		private float CurrentTempProgressSpeedFactor
		{
			get
			{
				float ambientTemperature = this.parent.AmbientTemperature;
				if (ambientTemperature < this.Product.temperatureSafe.min)
				{
					return this.Product.speedLessThanSafe;
				}
				if (ambientTemperature > this.Product.temperatureSafe.max)
				{
					return this.Product.speedMoreThanSafe;
				}
				if (ambientTemperature < this.Product.temperatureIdeal.min)
				{
					return GenMath.LerpDouble(this.Product.temperatureSafe.min, this.Product.temperatureIdeal.min, this.Product.speedLessThanSafe, 1f, ambientTemperature);
				}
				if (ambientTemperature > this.Product.temperatureIdeal.max)
				{
					return GenMath.LerpDouble(this.Product.temperatureIdeal.max, this.Product.temperatureSafe.max, 1f, this.Product.speedMoreThanSafe, ambientTemperature);
				}
				return 1f;
			}
		}

		public float SunRespectSpeedFactor
		{
			get
			{
				if (this.parent.Map == null)
				{
					return 0f;
				}
				if (this.Product.sunRespect.Span == 0f)
				{
					return 1f;
				}
				float x = this.parent.Map.skyManager.CurSkyGlow * (1f - this.RoofedFactor);
				return GenMath.LerpDouble(Static_Weather.SunGlowRange.TrueMin, Static_Weather.SunGlowRange.TrueMax, this.Product.sunRespect.min, this.Product.sunRespect.max, x);
			}
		}

		public float RainRespectSpeedFactor
		{
			get
			{
				if (this.parent.Map == null)
				{
					return 0f;
				}
				if (this.Product.rainRespect.Span == 0f)
				{
					return 1f;
				}
				if (this.parent.Map.weatherManager.SnowRate != 0f)
				{
					return this.Product.rainRespect.min;
				}
				float x = this.parent.Map.weatherManager.RainRate * (1f - this.RoofedFactor);
				return GenMath.LerpDouble(Static_Weather.RainRateRange.TrueMin, Static_Weather.RainRateRange.TrueMax, this.Product.rainRespect.min, this.Product.rainRespect.max, x);
			}
		}

		public float SnowRespectSpeedFactor
		{
			get
			{
				if (this.parent.Map == null)
				{
					return 0f;
				}
				if (this.Product.snowRespect.Span == 0f)
				{
					return 1f;
				}
				float x = this.parent.Map.weatherManager.SnowRate * (1f - this.RoofedFactor);
				return GenMath.LerpDouble(Static_Weather.SnowRateRange.TrueMin, Static_Weather.SnowRateRange.TrueMax, this.Product.snowRespect.min, this.Product.snowRespect.max, x);
			}
		}

		public float WindRespectSpeedFactor
		{
			get
			{
				if (this.parent.Map == null)
				{
					return 0f;
				}
				if (this.Product.windRespect.Span == 0f)
				{
					return 1f;
				}
				if (this.RoofedFactor != 0f)
				{
					return this.Product.windRespect.min;
				}
				return GenMath.LerpDouble(Static_Weather.WindSpeedRange.TrueMin, Static_Weather.WindSpeedRange.TrueMax, this.Product.windRespect.min, this.Product.windRespect.max, this.parent.Map.windManager.WindSpeed);
			}
		}

		public float RoofedFactor
		{
			get
			{
				if (this.parent.Map == null)
				{
					return 0f;
				}
				int num = 0;
				int num2 = 0;
				foreach (IntVec3 c in this.parent.OccupiedRect())
				{
					num++;
					if (this.parent.Map.roofGrid.Roofed(c))
					{
						num2++;
					}
				}
				return (float)num2 / (float)num;
			}
		}

		public float SpeedFactor
		{
			get
			{
				return Mathf.Max(this.CurrentTempProgressSpeedFactor * this.SunRespectSpeedFactor * this.RainRespectSpeedFactor * this.SnowRespectSpeedFactor * this.WindRespectSpeedFactor, 0f);
			}
		}

		private float CurrentProgressPerTick
		{
			get
			{
				return 1f / (float)this.Product.baseFermentationDuration * this.SpeedFactor;
			}
		}

		private int EstimatedTicksLeft
		{
			get
			{
				if (this.CurrentProgressPerTick == 0f)
				{
					return -1;
				}
				return Mathf.Max(Mathf.RoundToInt((1f - this.Progress) / this.CurrentProgressPerTick), 0);
			}
		}

		public bool Fueled
		{
			get
			{
				return this.refuelComp == null || this.refuelComp.HasFuel;
			}
		}

		public bool Powered
		{
			get
			{
				return this.powerTradeComp == null || this.powerTradeComp.PowerOn;
			}
		}

		public bool FlickedOn
		{
			get
			{
				return this.flickComp == null || this.flickComp.SwitchIsOn;
			}
		}

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			this.refuelComp = this.parent.GetComp<CompRefuelable>();
			this.powerTradeComp = this.parent.GetComp<CompPowerTrader>();
			this.flickComp = this.parent.GetComp<CompFlickable>();
			this.defaultTexPath = this.parent.def.graphicData.texPath;
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look<float>(ref this.ruinedPercent, "ruinedPercent", 0f, false);
			Scribe_Values.Look<int>(ref this.ingredientCount, "UF_UniversalFermenter_IngredientCount", 0, false);
			Scribe_Values.Look<float>(ref this.progressInt, "UF_UniversalFermenter_Progress", 0f, false);
			Scribe_Values.Look<int>(ref this.nextResourceInd, "UF_nextResourceInd", 0, false);
			Scribe_Values.Look<int>(ref this.currentResourceInd, "UF_currentResourceInd", 0, false);
			Scribe_Values.Look<string>(ref this.defaultTexPath, "defaultTexPath", null, false);
			Scribe_Collections.Look<string>(ref this.ingredientLabels, "UF_ingredientLabels", LookMode.Undefined, new object[0]);
		}

		public override void PostDraw()
		{
			base.PostDraw();
			if (!this.Empty)
			{
				Vector3 drawPos = this.parent.DrawPos;
				drawPos.y += 0.048387095f;
				drawPos.z += 0.25f;
				GenDraw.DrawFillableBar(new GenDraw.FillableBarRequest
				{
					center = drawPos,
					size = Static_Bar.Size,
					fillPercent = (float)this.ingredientCount / (float)this.Product.maxCapacity,
					filledMat = this.BarFilledMat,
					unfilledMat = Static_Bar.UnfilledMat,
					margin = 0.1f,
					rotation = Rot4.North
				});
			}
		}

		public bool AddIngredient(Thing ingredient)
		{
			if (!this.Product.ingredientFilter.Allows(ingredient))
			{
				return false;
			}
			if (!this.ingredientLabels.Contains(ingredient.def.label))
			{
				this.ingredientLabels.Add(ingredient.def.label);
			}
			this.AddIngredient(ingredient.stackCount);
			ingredient.Destroy(DestroyMode.Vanish);
			return true;
		}

		public void AddIngredient(int count)
		{
			this.ruinedPercent = 0f;
			if (this.Fermented)
			{
				Log.Warning("Universal Fermenter:: Tried to add ingredient to a fermenter full of product. Colonists should take the product first.", false);
				return;
			}
			int num = Mathf.Min(count, this.Product.maxCapacity - this.ingredientCount);
			if (num <= 0)
			{
				return;
			}
			this.Progress = GenMath.WeightedAverage(0f, (float)num, this.Progress, (float)this.ingredientCount);
			if (this.Empty)
			{
				this.GraphicChange(false);
			}
			this.ingredientCount += num;
		}

		public Thing TakeOutProduct()
		{
			if (!this.Fermented)
			{
				Log.Warning("Universal Fermenter:: Tried to get product but it's not yet fermented.", false);
				return null;
			}
			Thing thing = ThingMaker.MakeThing(this.Product.thingDef, null);
			thing.stackCount = Mathf.RoundToInt((float)this.ingredientCount * this.Product.efficiency);
			this.Reset();
			return thing;
		}

		public void Reset()
		{
			this.ingredientCount = 0;
			this.Progress = 0f;
			this.currentResourceInd = this.nextResourceInd;
			this.ingredientLabels.Clear();
			this.GraphicChange(true);
		}

		public void GraphicChange(bool toEmpty)
		{
			if (this.Product.graphSuffix != null)
			{
				string text = this.defaultTexPath;
				if (!toEmpty)
				{
					text += this.Product.graphSuffix;
				}
				TexReloader.Reload(this.parent, text);
			}
		}

		public override void CompTick()
		{
			base.CompTick();
			this.DoTicks(1);
		}

		public override void CompTickRare()
		{
			base.CompTickRare();
			this.DoTicks(250);
			if (!this.CheckGraphic && !this.Empty)
			{
				this.CheckGraphic = true;
				this.GraphicChange(false);
			}
		}

		private void DoTicks(int ticks)
		{
			if (!this.Empty && this.Fueled && this.Powered && this.FlickedOn)
			{
				this.Progress = Mathf.Min(this.Progress + (float)ticks * this.CurrentProgressPerTick, 1f);
			}
			if (!this.Ruined)
			{
				if (!this.Empty)
				{
					float ambientTemperature = this.parent.AmbientTemperature;
					if (ambientTemperature > this.Product.temperatureSafe.max)
					{
						this.ruinedPercent += (ambientTemperature - this.Product.temperatureSafe.max) * this.Product.progressPerDegreePerTick * (float)ticks;
					}
					else if (ambientTemperature < this.Product.temperatureSafe.min)
					{
						this.ruinedPercent -= (ambientTemperature - this.Product.temperatureSafe.min) * this.Product.progressPerDegreePerTick * (float)ticks;
					}
				}
				if (this.ruinedPercent >= 1f)
				{
					this.ruinedPercent = 1f;
					this.parent.BroadcastCompSignal("RuinedByTemperature");
					this.Reset();
					return;
				}
				if (this.ruinedPercent < 0f)
				{
					this.ruinedPercent = 0f;
				}
			}
		}

		public override void PreAbsorbStack(Thing otherStack, int count)
		{
			float num = (float)count / (float)(this.parent.stackCount + count);
			CompUniversalFermenter comp = ((ThingWithComps)otherStack).GetComp<CompUniversalFermenter>();
			this.ruinedPercent = Mathf.Lerp(this.ruinedPercent, comp.ruinedPercent, num);
		}

		public override bool AllowStackWith(Thing other)
		{
			CompUniversalFermenter comp = ((ThingWithComps)other).GetComp<CompUniversalFermenter>();
			return this.Ruined == comp.Ruined;
		}

		public override void PostSplitOff(Thing piece)
		{
			((ThingWithComps)piece).GetComp<CompUniversalFermenter>().ruinedPercent = this.ruinedPercent;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Prefs.DevMode && !this.Empty)
			{
				Command_Action command_Action = new Command_Action
				{
					defaultLabel = "DEBUG: Finish",
					activateSound = SoundDef.Named("Click"),
					action = delegate()
					{
						this.Progress = 1f;
					}
				};
				yield return command_Action;
			}
			if (Prefs.DevMode)
			{
				string line = string.Concat(new string[]
				{
					this.parent.ToString(),
					": sun: ",
					this.SunRespectSpeedFactor.ToString("0.00"),
					", rain: ",
					this.RainRespectSpeedFactor.ToString("0.00"),
					", snow: ",
					this.SnowRespectSpeedFactor.ToString("0.00"),
					", wind: ",
					this.WindRespectSpeedFactor.ToString("0.00"),
					", roofed: ",
					this.RoofedFactor.ToString("0.00")
				});
				Command_Action command_Action2 = new Command_Action
				{
					defaultLabel = "DEBUG: Display Speed Factors",
					defaultDesc = "Display the current sun, rain, snow and wind speed factors and how much of the building is covered by roof.",
					activateSound = SoundDef.Named("Click"),
					action = delegate()
					{
						Log.Message(line, false);
					}
				};
				yield return command_Action2;
			}
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
			IEnumerator<Gizmo> enumerator = null;
			if (this.ResourceListSize > 1)
			{
				Command_Action command_Action3 = new Command_Action
				{
					defaultLabel = this.NextProduct.thingDef.label,
					defaultDesc = string.Concat(new string[]
					{
						"Produce ",
						this.NextProduct.thingDef.label,
						" from ",
						this.SummaryNextIngredientFilter,
						"."
					}),
					activateSound = SoundDef.Named("Click"),
					icon = Utils.GetIcon(this.NextProduct.thingDef),
					action = delegate()
					{
						this.NextResource();
					}
				};
				yield return command_Action3;
			}
			yield break;
			yield break;
		}

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.StatusInfo());
			if (!this.Empty && !this.Ruined)
			{
				if (this.Fermented)
				{
					stringBuilder.AppendLine("UF_ContainsProduct".Translate(new object[]
					{
						this.ingredientCount,
						this.Product.maxCapacity,
						this.Product.thingDef.label
					}));
				}
				else
				{
					stringBuilder.AppendLine("UF_ContainsIngredient".Translate(new object[]
					{
						this.ingredientCount,
						this.Product.maxCapacity,
						this.SummaryAddedIngredients
					}));
				}
			}
			if (!this.Empty)
			{
				if (this.Fermented)
				{
					stringBuilder.AppendLine("UF_Finished".Translate());
				}
				else if (this.parent.Map != null)
				{
					stringBuilder.AppendLine("UF_Progress".Translate(new object[]
					{
						this.Progress.ToStringPercent(),
						this.TimeLeft()
					}));
					if (this.SpeedFactor != 1f)
					{
						if (this.SpeedFactor < 1f)
						{
							stringBuilder.Append("UF_NonIdealInfluences".Translate(new object[]
							{
								this.WhatsWrong()
							})).Append(" ").AppendLine("UF_NonIdealSpeedFactor".Translate(new object[]
							{
								this.SpeedFactor.ToStringPercent()
							}));
						}
						else
						{
							stringBuilder.AppendLine("UF_NonIdealSpeedFactor".Translate(new object[]
							{
								this.SpeedFactor.ToStringPercent()
							}));
						}
					}
				}
			}
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				"UF_IdealSafeProductionTemperature".Translate(),
				": ",
				this.Product.temperatureIdeal.min.ToStringTemperature("F0"),
				"~",
				this.Product.temperatureIdeal.max.ToStringTemperature("F0"),
				" (",
				this.Product.temperatureSafe.min.ToStringTemperature("F0"),
				"~",
				this.Product.temperatureSafe.max.ToStringTemperature("F0"),
				")"
			}));
			return stringBuilder.ToString().TrimEndNewlines();
		}

		public string TimeLeft()
		{
			if (this.EstimatedTicksLeft >= 0)
			{
				return this.EstimatedTicksLeft.ToStringTicksToPeriod() + " left";
			}
			return "stopped";
		}

		public string WhatsWrong()
		{
			if (this.SpeedFactor < 1f)
			{
				List<string> list = new List<string>();
				if (this.CurrentTempProgressSpeedFactor < 1f)
				{
					list.Add("UF_WeatherTemperature".Translate());
				}
				if (this.SunRespectSpeedFactor < 1f)
				{
					list.Add("UF_WeatherSunshine".Translate());
				}
				if (this.RainRespectSpeedFactor < 1f)
				{
					list.Add("UF_WeatherRain".Translate());
				}
				if (this.SnowRespectSpeedFactor < 1f)
				{
					list.Add("UF_WeatherSnow".Translate());
				}
				if (this.WindRespectSpeedFactor < 1f)
				{
					list.Add("UF_WeatherWind".Translate());
				}
				return string.Join(", ", list.ToArray());
			}
			return "nothing";
		}

		public string StatusInfo()
		{
			if (this.Ruined)
			{
				return "RuinedByTemperature".Translate();
			}
			float ambientTemperature = this.parent.AmbientTemperature;
			string text = null;
			string text2 = "Temperature".Translate() + ": " + ambientTemperature.ToStringTemperature("F0");
			if (!this.Empty)
			{
				if (this.Product.temperatureSafe.Includes(ambientTemperature))
				{
					if (this.Product.temperatureIdeal.Includes(ambientTemperature))
					{
						text = "UF_Ideal".Translate();
					}
					else
					{
						text = "UF_Safe".Translate();
					}
				}
				else if (this.ruinedPercent > 0f)
				{
					if (ambientTemperature < this.Product.temperatureSafe.min)
					{
						text = "Freezing".Translate();
					}
					else
					{
						text = "Overheating".Translate();
					}
					text = text + " " + this.ruinedPercent.ToStringPercent();
				}
			}
			if (text == null)
			{
				return text2;
			}
			return text2 + " (" + text + ")";
		}

		private int ingredientCount;

		private float progressInt;

		private Material barFilledCachedMat;

		private int nextResourceInd;

		private int currentResourceInd;

		private List<string> ingredientLabels = new List<string>();

		protected float ruinedPercent;

		public string defaultTexPath;

		public CompRefuelable refuelComp;

		public CompPowerTrader powerTradeComp;

		public CompFlickable flickComp;

		public const string RuinedSignal = "RuinedByTemperature";

		public bool CheckGraphic;
	}
}
