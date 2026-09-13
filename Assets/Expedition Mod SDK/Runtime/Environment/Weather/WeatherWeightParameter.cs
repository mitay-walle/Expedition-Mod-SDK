using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Environment.Weather
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class WeatherWeightParameter : VolumeParameter<List<WeatherWeight>>
	{
		private readonly List<WeatherWeight> _first = new();
		private readonly List<WeatherWeight> _second = new();

		public WeatherWeightParameter() : base(new List<WeatherWeight>())
		{
		}

		public void Blend(WeatherDefinition definition, float influence)
		{
			List<WeatherWeight> result = ReferenceEquals(value, _first) ? _second : _first;
			result.Clear();
			result.AddRange(value);
			int index = -1;
			for (int candidate = 0; candidate < result.Count; candidate++)
			{
				if (result[candidate].Definition == definition)
				{
					index = candidate;
					break;
				}
			}
			float previous = index < 0 ? 0f : result[index].Weight;
			var blended = new WeatherWeight(definition, Mathf.Lerp(previous, 1f, influence));
			if (index < 0)
			{
				result.Add(blended);
			}
			else
			{
				result[index] = blended;
			}
			value = result;
		}

	}
}
