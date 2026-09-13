using System;
using UnityEngine;

namespace Game.Environment.Weather
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public struct WeatherWeight
	{
		public WeatherDefinition Definition;
		[Range(0f, 1f)] public float Weight;

		public WeatherWeight(WeatherDefinition definition, float weight)
		{
			Definition = definition;
			Weight = Mathf.Clamp01(weight);
		}
	}
}
