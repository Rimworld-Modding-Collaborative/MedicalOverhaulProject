using System;
using System.Reflection;
using Verse;

namespace MedicalOverhaul
{
	//public static class TexReloader
	//{
	//	public static void Reload(Thing t, string texPath)
	//	{
	//		Graphic value = GraphicDatabase.Get(t.def.graphicData.graphicClass, texPath, ShaderDatabase.LoadShader(t.def.graphicData.shaderType.shaderPath), t.def.graphicData.drawSize, t.DrawColor, t.DrawColorTwo);
	//		typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(t, value);
	//		if (t.Map != null)
	//		{
	//			t.DirtyMapMesh(t.Map);
	//		}
	//	}
	//}
}
