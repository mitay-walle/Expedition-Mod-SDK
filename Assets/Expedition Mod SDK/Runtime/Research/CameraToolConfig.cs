using System;
using UnityEngine;

namespace Research
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class CameraToolConfig : ResearchToolConfig
	{
		[field: SerializeField, Min(1f)]
		public float Range { get; private set; } = 20f;
		[field: SerializeField, Range(0.01f, 1f)]
		public float MinimumFrameArea { get; private set; } = 0.05f;
		public override string ReadyMessage => "Samples.PhotoReady";
		public override void Capture(IResearchCapture capture) => capture.TakePhoto(this);
	}
}
