using System;
using UnityEngine.Rendering;

namespace Systems.Environment.Winds
{
	[Serializable, VolumeComponentMenu("Environment/Wind")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class WindVolumeComponent : VolumeComponent
	{
		public ClampedFloatParameter ExternalIntensity = new(0.152f, 0f, 1f);
	}
}