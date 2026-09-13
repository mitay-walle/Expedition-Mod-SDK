using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Environment
{
	[Serializable, VolumeComponentMenu("Environment/Gravity")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class GravityVolume : VolumeComponent
	{
		[Tooltip("Gravity multiplier: 0 disables gravity, 1 keeps normal gravity, values above 1 increase it.")]
		public MinFloatParameter GravityFactor = new(1f, 0f);
	}
}
