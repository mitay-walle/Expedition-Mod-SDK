using System;
using UnityEngine.Rendering;

namespace Game.Environment.Weather
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class WeatherDefinitionParameter : VolumeParameter<WeatherDefinition>
	{
		public WeatherDefinitionParameter() : base(null)
		{
		}
	}
}
