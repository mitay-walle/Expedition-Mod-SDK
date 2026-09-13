using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
	[CreateAssetMenu(menuName = "Audio/Layered Audio Clip")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Plugins")]
	public sealed class LayeredAudioClip : ScriptableObject
	{
		[field: SerializeField] public List<LayeredAudioClipLayer> Layers { get; private set; } = new();

		public bool HasPlayableClips()
		{
			foreach (LayeredAudioClipLayer layer in Layers)
			{
				if (layer != null && layer.HasClips()) return true;
			}

			return false;
		}
	}
}