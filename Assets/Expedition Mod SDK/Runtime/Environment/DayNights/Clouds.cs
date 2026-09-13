using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Environment.DayNights
{
	[Serializable, VolumeComponentMenu("Environment/Clouds")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class Clouds : VolumeComponent
	{
		[Tooltip("Cloud Appearance material. Use the same material across profiles for a shared look; separate materials select different looks without blending their properties.")]
		public MaterialParameter Appearance = new(null);
		public ColorParameter Tint = new(Color.white);
		[Tooltip("0 = clear sky; increasing Coverage adds clouds. Density controls opacity.")]
		public ClampedFloatParameter Coverage = new(0.55f, 0f, 1f);
		public ClampedFloatParameter Cirrus = new(0.2f, 0f, 1f);
		public ClampedFloatParameter Cirrocumulus = new(0f, 0f, 1f);
		public ClampedFloatParameter Cirrostratus = new(0f, 0f, 1f);
		public ClampedFloatParameter Altocumulus = new(0f, 0f, 1f);
		public ClampedFloatParameter Altostratus = new(0f, 0f, 1f);
		public ClampedFloatParameter Stratocumulus = new(0f, 0f, 1f);
		public ClampedFloatParameter Stratus = new(0f, 0f, 1f);
		public ClampedFloatParameter Nimbostratus = new(0f, 0f, 1f);
		public ClampedFloatParameter Cumulus = new(1f, 0f, 1f);
		public ClampedFloatParameter Cumulonimbus = new(0f, 0f, 1f);
		public ClampedFloatParameter Density = new(0.7f, 0f, 2f);
		public ClampedFloatParameter Speed = new(0.01f, -1f, 1f);
	}
}
