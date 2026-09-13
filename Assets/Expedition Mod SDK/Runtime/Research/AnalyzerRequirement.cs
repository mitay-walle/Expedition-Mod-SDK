using System;
using UnityEngine;
namespace Research
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class AnalyzerRequirement
	{
		 public string TypeId;
		[Min(0.1f)] public float Delay = 30f;
		[SerializeReference] public AnalyzerReadings Readings;
		public void Validate()
		{
			if (Readings == null || TypeId != Readings.TypeId || !float.IsFinite(Delay) || Delay <= 0)
			{
				throw new InvalidOperationException("Analyzer requirement type or delay is invalid.");
			}
			Readings.Validate();
		}
	}
}
