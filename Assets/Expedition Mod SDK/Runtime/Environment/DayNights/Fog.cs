using System;
using UnityEngine.Rendering;

namespace Game.Environment.DayNights
{
	[Serializable, VolumeComponentMenu("Environment/Fog")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class Fog : VolumeComponent
	{
		public BoolParameter Enabled = new(false);
		public ColorParameter Color = new(UnityEngine.Color.gray);
		public FogModeParameter Mode = new(UnityEngine.FogMode.ExponentialSquared);
		public ClampedFloatParameter Density = new(0.01f, 0f, 1f);
		public FloatParameter StartDistance = new(0f);
		public FloatParameter EndDistance = new(300f);
	}
}