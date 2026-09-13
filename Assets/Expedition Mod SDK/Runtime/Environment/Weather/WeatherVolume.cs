using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Environment.Weather
{
	[Serializable, VolumeComponentMenu("Environment/Weather")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class WeatherVolume : VolumeComponent
	{
		public WeatherDefinitionParameter Definition = new();
		[NonSerialized, HideInInspector]
		public WeatherWeightParameter Weights = new();

		public override void Override(VolumeComponent state, float interpFactor)
		{
			if (!Definition.overrideState || !Definition.value)
			{
				return;
			}
			var weather = (WeatherVolume)state;
			weather.Weights.overrideState = true;
			weather.Weights.Blend(Definition.value, interpFactor);
		}
	}
}
