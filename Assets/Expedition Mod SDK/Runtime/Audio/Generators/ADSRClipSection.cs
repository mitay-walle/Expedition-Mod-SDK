using System;
using UnityEngine;

namespace Audio.Generators
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public class ADSRClipSection
	{
		[SerializeField] private AudioClip _clip;
		[SerializeField, Delayed, Min(0f)] private float _startTime;
		[SerializeField, Delayed, Min(0f)] private float _endTime;

		public AudioClip Clip => _clip;
		public float StartTime => _startTime;
		public float EndTime => _endTime;
	}
}