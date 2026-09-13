using System;
using UnityEngine.Rendering;

namespace Game.Environment.DayNights
{
	[Serializable, VolumeComponentMenu("Environment/Moon")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class Moon : VolumeComponent
	{
		public TextureParameter Texture = new(null);
		public ClampedFloatParameter Size = new(0.03f, 0.001f, 0.2f);
		public MinFloatParameter Intensity = new(0f, 0f);
	}
}