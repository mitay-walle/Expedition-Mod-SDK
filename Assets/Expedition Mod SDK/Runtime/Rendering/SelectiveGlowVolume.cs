using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Rendering
{
	[Serializable, VolumeComponentMenu("Rendering/Selective Glow")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class SelectiveGlowVolume : VolumeComponent, IPostProcessComponent
	{
		public BoolParameter Enabled = new(false);
		public EnumParameter<SelectiveGlowResolution> Resolution = new(SelectiveGlowResolution.Quarter);
		public ClampedFloatParameter BlurRadius = new(2f, 0.5f, 20f);
		public ClampedIntParameter BlurIterations = new(2, 1, 6);
		public ClampedFloatParameter Intensity = new(1f, 0f, 16f);

		public bool IsActive()
		{
			return Enabled.value && Intensity.value > 0f;
		}

		public bool IsTileCompatible()
		{
			return false;
		}
	}
}
