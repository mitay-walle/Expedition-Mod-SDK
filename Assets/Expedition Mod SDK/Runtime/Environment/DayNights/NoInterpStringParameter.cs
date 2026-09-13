using System;
using UnityEngine.Rendering;

namespace Game.Environment.DayNights
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class NoInterpStringParameter : VolumeParameter<string>
	{
		public NoInterpStringParameter(string value, bool overrideState = false) : base(value, overrideState)
		{
		}
	}
}
