using System;
using UnityEngine.Rendering;

namespace Game.Environment.DayNights
{
	[Serializable, VolumeComponentMenu("Environment/Sun")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class Sun : VolumeComponent
	{
		public ColorParameter Color = new(UnityEngine.Color.white);
		public MinFloatParameter Intensity = new(1f, 0f);
		public ClampedFloatParameter ShadowStrength = new(1f, 0f, 1f);
		public ClampedFloatParameter DiskIntensity = new(1f, 0f, 1f);
		public ClampedFloatParameter Size = new(0.019f, 0.001f, 1f);
		public ClampedFloatParameter SizeConvergence = new(1f, 1f, 10f);
	}
}