using UnityEngine;

namespace Game.Environment.DayNights
{
	[CreateAssetMenu(menuName = "Environment/Celestial Body")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class CelestialBodyDefinition : ScriptableObject
	{
		[field: SerializeField] public Texture2D Texture { get; private set; }
		[field: SerializeField, InspectorName("Sphere (off = Disk)"), Tooltip("Sphere: longitude/latitude texture and spherical lighting. Disk: flat RGBA image facing the viewer.")]
		public bool Sphere { get; private set; } = true;
		[field: SerializeField, ColorUsage(true, true)] public Color Color { get; private set; } = Color.white;
		[field: SerializeField, ColorUsage(true, true)] public Color EdgeColor { get; private set; } = Color.gray;
		[field: SerializeField, Min(0f)] public float Intensity { get; private set; } = 1f;
		[field: SerializeField, Range(0f, 1f)] public float Emission { get; private set; }
		[field: SerializeField, Range(0f, 1f)] public float RingSize { get; private set; }
		[field: SerializeField, Range(0f, 360f)] public float TextureRotation { get; private set; }
	}
}
