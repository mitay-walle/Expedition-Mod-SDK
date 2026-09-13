using System;
using UnityEngine.Rendering;

namespace Game.Environment.DayNights
{
	[Serializable, VolumeComponentMenu("Environment/Ambient")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class Ambient : VolumeComponent
	{
		public FloatParameter ReflectionProbeIntensity = new(1f);
		public ClampedFloatParameter ReflectionIntensity = new(1f, 0f, 1f);
		public ClampedFloatParameter Intensity = new(0.5f, 0f, 8f);
		public ColorParameter LightSourceColor = new(UnityEngine.Color.white);
	}
}