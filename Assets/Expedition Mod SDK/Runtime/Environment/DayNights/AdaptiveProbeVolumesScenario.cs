using System;
using UnityEngine.Rendering;

namespace Game.Environment.DayNights
{
	[Serializable, VolumeComponentMenu("Lighting/Adaptive Probe Volumes Scenario")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class AdaptiveProbeVolumesScenario : VolumeComponent
	{
		public NoInterpStringParameter Scenario = new("Default");
	}
}
