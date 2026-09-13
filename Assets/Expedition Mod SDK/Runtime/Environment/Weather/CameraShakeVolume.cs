using System;
using UnityEngine.Rendering;

namespace Game.Environment.Weather
{
	[Serializable, VolumeComponentMenu("Environment/Camera Shake")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class CameraShakeVolume : VolumeComponent
	{
		public ClampedFloatParameter PositionAmplitude = new(0, 0, 0.5f);
		public ClampedFloatParameter RotationAmplitude = new(0, 0, 5);
		public ClampedFloatParameter Frequency = new(8, 0.1f, 20);
	}
}
