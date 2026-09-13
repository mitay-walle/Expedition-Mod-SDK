using System;
using UnityEngine;

namespace Game.Environment.Weather
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class WeatherEventSchedule
	{
		[field: SerializeField]
		public string Id { get; private set; }
		[field: SerializeField] public Vector2 Interval { get; private set; } = new(4f, 12f);
	}
}

