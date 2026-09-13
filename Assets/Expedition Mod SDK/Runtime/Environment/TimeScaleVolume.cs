using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Environment
{
	[Serializable, VolumeComponentMenu("Environment/Time Scale")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class TimeScaleVolume : VolumeComponent
	{
		[Tooltip("Global time scale: 0 stops time, 1 is normal speed, values above 1 speed up time.")]
		public MinFloatParameter TimeScale = new(1f, 0f);
	}
}
