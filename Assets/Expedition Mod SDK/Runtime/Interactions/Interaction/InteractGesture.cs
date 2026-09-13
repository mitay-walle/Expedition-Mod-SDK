using System;
using UnityEngine;

namespace Interactions.Interaction
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class InteractGesture
	{
		public InteractGestureMode Mode = InteractGestureMode.SingleClick;
		[Min(1)] public int RequiredCount = 1;
		[Min(0.01f)] public float Duration = 1;
		[Min(0.01f)] public float TickInterval = 0.25f;

		public void Validate()
		{
			if (RequiredCount <= 0 || Duration <= 0 || TickInterval <= 0)
			{
				throw new InvalidOperationException("Interaction gesture values must be positive.");
			}
		}
	}
}
