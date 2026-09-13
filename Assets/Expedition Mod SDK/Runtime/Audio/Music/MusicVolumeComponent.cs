using System;
using UnityEngine.Rendering;

namespace Game.Audio
{
	[Serializable, VolumeComponentMenu("Audio/Music")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class MusicVolumeComponent : VolumeComponent
	{
		public EnumParameter<MusicType> Music = new(MusicType.Discovery);
	}
}