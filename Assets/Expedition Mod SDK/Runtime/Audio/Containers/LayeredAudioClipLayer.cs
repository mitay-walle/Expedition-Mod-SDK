using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Plugins")]
	public sealed class LayeredAudioClipLayer
	{
		[field: SerializeField] public List<AudioClip> Clips { get; private set; } = new();
		[field: SerializeField] public Vector2 DelayRange { get; private set; }
		[field: SerializeField] public float TrimStart { get; private set; }
		[field: SerializeField] public float TrimEnd { get; private set; }
		[field: SerializeField] public Vector2 VolumeRange { get; private set; } = Vector2.one;
		[field: SerializeField] public Vector2 PitchRange { get; private set; } = Vector2.one;

		public bool HasClips()
		{
			foreach (AudioClip clip in Clips)
			{
				if (clip != null) return true;
			}

			return false;
		}

		public bool TryRoll(out AudioClip clip, out float delay, out float trimStart, out float trimEnd, out float volume, out float pitch)
		{
			clip = RollClip();
			delay = Roll(DelayRange);
			trimStart = GetTrimStart(clip);
			trimEnd = GetTrimEnd(clip);
			volume = Roll(VolumeRange);
			pitch = Roll(PitchRange);
			return clip != null && GetTrimmedLength(clip) > 0f;
		}

		public float GetTrimStart(AudioClip clip)
		{
			if (clip == null) return 0f;

			return Mathf.Clamp(TrimStart, 0f, clip.length);
		}

		public float GetTrimEnd(AudioClip clip)
		{
			if (clip == null) return 0f;

			return Mathf.Clamp(TrimEnd, 0f, Mathf.Max(0f, clip.length - GetTrimStart(clip)));
		}

		public float GetTrimmedLength(AudioClip clip)
		{
			if (clip == null) return 0f;

			return Mathf.Max(0f, clip.length - GetTrimStart(clip) - GetTrimEnd(clip));
		}

		private AudioClip RollClip()
		{
			int clipCount = 0;
			foreach (AudioClip clip in Clips)
			{
				if (clip != null) clipCount++;
			}

			if (clipCount == 0) return null;

			int selected = UnityEngine.Random.Range(0, clipCount);
			foreach (AudioClip clip in Clips)
			{
				if (clip == null) continue;
				if (selected == 0) return clip;

				selected--;
			}

			return null;
		}

		private float Roll(Vector2 range)
		{
			float min = Mathf.Min(range.x, range.y);
			float max = Mathf.Max(range.x, range.y);
			return UnityEngine.Random.Range(min, max);
		}
	}
}