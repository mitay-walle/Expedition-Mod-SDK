using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Research
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class AtmosphereReadings : AnalyzerReadings
	{
		public override string TypeId => "Atmosphere";
		public float Temperature;
		[Range(0f, 100f)] public float Pollution;
		public List<AtmosphereGas> Gases = new();
		[Min(0f)] public float Radiation;
		public override void Validate()
		{
			if (Conclusion == null || Conclusion.IsEmpty || !float.IsFinite(Temperature) || !float.IsFinite(Pollution) || Pollution < 0 || Pollution > 100 || !float.IsFinite(Radiation) || Radiation < 0 || Gases.Count == 0 || Gases.Any(g => g == null || g.Name.IsEmpty || !float.IsFinite(g.Percentage) || g.Percentage < 0 || g.Percentage > 100) || Mathf.Abs(Gases.Sum(g => g.Percentage) - 100) > 0.01f)
			{
				throw new InvalidOperationException("Invalid atmosphere readings; gas percentages must total 100.");
			}
		}
		public override IEnumerable<string> Describe()
		{
			yield return Text("Analyzers.Temperature", Temperature);
			yield return Text("Analyzers.Pollution", Pollution);
			foreach (AtmosphereGas gas in Gases)
			{
				yield return Text("Analyzers.Gas", gas.Name.GetLocalizedString(), gas.Percentage);
			}
			yield return Text("Analyzers.Radiation", Radiation);
			yield return Conclusion.GetLocalizedString();
		}
		public override AnalyzerReadings Copy() => new AtmosphereReadings { Temperature = Temperature, Pollution = Pollution, Radiation = Radiation, Conclusion = CopyText(Conclusion), Gases = Gases.Select(g => new AtmosphereGas { Name = CopyText(g.Name), Percentage = g.Percentage }).ToList() };
	}
}
