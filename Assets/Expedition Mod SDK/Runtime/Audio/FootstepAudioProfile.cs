using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
	[CreateAssetMenu(menuName = "Audio/Footstep Audio Profile")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class FootstepAudioProfile : ScriptableObject
	{
		[field: SerializeField] public LayeredAudioClip DefaultSound { get; private set; }
		[field: SerializeField] public List<FootstepSurfaceSound> Surfaces { get; private set; } = new();

		public LayeredAudioClip GetSound(PhysicsMaterial material, TerrainLayer terrainLayer)
		{
			foreach (FootstepSurfaceSound surface in Surfaces)
			{
				if (terrainLayer != null ? surface.TerrainLayer == terrainLayer : material != null && surface.PhysicsMaterial == material)
				{
					return surface.Sound;
				}
			}

			return DefaultSound;
		}
	}
}