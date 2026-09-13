using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
namespace Research
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class GeologyReadings : AnalyzerReadings
	{
		public override string TypeId => "Geology";
		public EarthquakeRisk EarthquakeRisk;
		public List<LocalizedString> Anomalies = new();
		[Min(0f)] public float FaultDistance;
		public override void Validate()
		{
			if (Conclusion == null || Conclusion.IsEmpty || !Enum.IsDefined(typeof(EarthquakeRisk), EarthquakeRisk) || !float.IsFinite(FaultDistance) || FaultDistance < 0 || Anomalies.Exists(a => a == null || a.IsEmpty))
			{
				throw new InvalidOperationException("Invalid geological readings.");
			}
		}
		public override IEnumerable<string> Describe()
		{
			yield return Text("Analyzers.Earthquake", Text("Analyzers.Risk." + EarthquakeRisk));
			yield return Text("Analyzers.FaultDistance", FaultDistance);
			yield return Text("Analyzers.Anomalies");
			if (Anomalies.Count == 0)
			{
				yield return Text("Analyzers.NoAnomalies");
			}
			foreach (LocalizedString anomaly in Anomalies)
			{
				yield return anomaly.GetLocalizedString();
			}
			yield return Conclusion.GetLocalizedString();
		}
		public override AnalyzerReadings Copy() => new GeologyReadings { EarthquakeRisk = EarthquakeRisk, FaultDistance = FaultDistance, Conclusion = CopyText(Conclusion), Anomalies = Anomalies.Select(CopyText).ToList() };
	}
}
