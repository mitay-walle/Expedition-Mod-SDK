using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

namespace Game.Environment.Weather
{
	[CreateAssetMenu(menuName = "Game/Environment/Weather", fileName = "Weather")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class WeatherDefinition : ScriptableObject
	{
		[field: SerializeField]
		public string Id { get; private set; }
		[field: SerializeField]
		public List<string> Tags { get; private set; } = new();
		[field: SerializeField, InspectorName("Color"), ColorUsage(false), Tooltip("Display color for this weather in the timeline. Does not tint the atmosphere or effects.")]
		public Color TimelineColor { get; private set; } = new(0.25f, 0.32f, 0.38f, 1f);
		[field: SerializeField, Min(0)] public float Weight { get; private set; } = 1f;
		[field: SerializeField] public Vector2 HoldDuration { get; private set; } = new(60, 120);
		[field: SerializeField, Min(0.1f)] public float EntryDuration { get; private set; } = 5f;
		[field: SerializeField, Min(0.1f)] public float ExitDuration { get; private set; } = 5f;
		[field: SerializeField, Min(0)] public float Cooldown { get; private set; } = 60f;
		[field: SerializeField] public VolumeProfile Profile { get; private set; }
		[field: SerializeField] public GameObject Effects { get; private set; }
		[field: SerializeField] public TimelineAsset Entry { get; private set; }
		[field: SerializeField] public TimelineAsset Hold { get; private set; }
		[field: SerializeField] public TimelineAsset Exit { get; private set; }
		[field: SerializeField]
		public List<string> RequiredMilestones { get; private set; } = new();
		[field: SerializeField]
		public List<string> RequiredFlags { get; private set; } = new();
		[field: SerializeField]
		public List<string> ForbiddenFlags { get; private set; } = new();
		[field: SerializeField] public List<WeatherEventSchedule> Events { get; private set; } = new();
		[field: SerializeField, Tooltip("Request this episode once when its progression requirements become satisfied.")]
		public bool RequestOnUnlock { get; private set; }
		[field: SerializeField] public bool InterruptOnUnlock { get; private set; }
		[field: SerializeField] public int UnlockPriority { get; private set; }
	}
}

