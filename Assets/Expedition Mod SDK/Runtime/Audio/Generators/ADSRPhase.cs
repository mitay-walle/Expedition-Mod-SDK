using System;
using UnityEngine;

namespace Audio.Generators
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public class ADSRPhase
	{
		[SerializeField, Min(0f)] private float _duration = 0.1f;
		[SerializeField] private AnimationCurve _volume = AnimationCurve.Constant(0f, 1f, 1f);
		[SerializeField] private AnimationCurve _pitch = AnimationCurve.Constant(0f, 1f, 1f);

		public float Duration => _duration;
		public AnimationCurve Volume => _volume;
		public AnimationCurve Pitch => _pitch;
	}
}