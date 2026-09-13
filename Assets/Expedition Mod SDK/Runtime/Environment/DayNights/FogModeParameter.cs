using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Environment.DayNights
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class FogModeParameter : VolumeParameter<FogMode>
	{
		public FogModeParameter(FogMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
