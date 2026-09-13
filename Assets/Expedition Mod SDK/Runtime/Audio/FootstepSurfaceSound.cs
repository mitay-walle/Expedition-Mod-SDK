using System;
using UnityEngine;

namespace Game.Audio
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class FootstepSurfaceSound
	{
		[field: SerializeField] public PhysicsMaterial PhysicsMaterial { get; private set; }
		[field: SerializeField] public TerrainLayer TerrainLayer { get; private set; }
		[field: SerializeField] public LayeredAudioClip Sound { get; private set; }
	}
}