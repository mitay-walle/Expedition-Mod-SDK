using System;
using UnityEngine.Rendering;

namespace Game.Environment.DayNights
{
	[Serializable, VolumeComponentMenu("Environment/Skybox")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class Skybox : VolumeComponent
	{
		public ClampedFloatParameter AtmosphereThickness = new(1.5f, 0f, 5f);
		public ClampedFloatParameter Exposure = new(1f, 0f, 8f);
		public ColorParameter AtmosphereTint = new(UnityEngine.Color.white);
		public ColorParameter HorizonTint = new(UnityEngine.Color.white);
		public ColorParameter SunsetTint = new(new UnityEngine.Color(1f, 0.22f, 0.06f, 1f));
		public ClampedFloatParameter SunsetIntensity = new(1.5f, 0f, 8f);
		public ClampedFloatParameter SunsetWidth = new(0.18f, 0.01f, 1f);
		public ColorParameter NightTint = new(new UnityEngine.Color(0.015f, 0.025f, 0.08f, 1f));
		public TextureParameter StarTexture = new(null);
		public ClampedFloatParameter StarIntensity = new(1.2f, 0f, 8f);
		public ClampedFloatParameter StarDensity = new(0.55f, 0f, 1f);
		public ColorParameter StarTint = new(UnityEngine.Color.white);
	}
}
