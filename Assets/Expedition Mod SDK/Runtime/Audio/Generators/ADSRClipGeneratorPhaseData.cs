using Unity.Collections;

namespace Audio.Generators
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public struct ADSRClipGeneratorPhaseData
	{
		public float Duration;
		public ADSRCurveTable Volume;
		public ADSRCurveTable Pitch;

		public static ADSRClipGeneratorPhaseData Create(ADSRPhase phase, Allocator allocator)
		{
			return new ADSRClipGeneratorPhaseData
			{
				Duration = phase != null ? phase.Duration : 0f,
				Volume = ADSRCurveTable.Create(phase != null ? phase.Volume : null, allocator),
				Pitch = ADSRCurveTable.Create(phase != null ? phase.Pitch : null, allocator)
			};
		}

		public void Dispose()
		{
			Volume.Dispose();
			Pitch.Dispose();
		}
	}
}