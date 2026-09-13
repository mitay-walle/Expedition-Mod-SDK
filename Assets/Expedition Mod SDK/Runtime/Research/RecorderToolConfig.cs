using System;
using UnityEngine;

namespace Research
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class RecorderToolConfig : ResearchToolConfig
	{
		[field: SerializeField, Min(1f)]
		public float Range { get; private set; } = 15f;
		[field: SerializeField, Min(1f)]
		public float RequiredDuration { get; private set; } = 5f;
		[field: SerializeField, Min(1f)]
		public float MaximumDuration { get; private set; } = 15f;
		public override string ReadyMessage => "Samples.RecordReady";
		public override void Capture(IResearchCapture capture) => capture.ToggleRecording(this);
	}
}
