using Unity.Collections;

namespace Audio.Generators
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public struct ADSRClipGeneratorPhaseSet
	{
		public ADSRClipGeneratorPhaseData Attack;
		public ADSRClipGeneratorPhaseData Decay;
		public ADSRClipGeneratorPhaseData Sustain;
		public ADSRClipGeneratorPhaseData Release;

		public static ADSRClipGeneratorPhaseSet Create(ADSRPhase attack, ADSRPhase decay, ADSRPhase sustain, ADSRPhase release, Allocator allocator)
		{
			return new ADSRClipGeneratorPhaseSet
			{
				Attack = ADSRClipGeneratorPhaseData.Create(attack, allocator),
				Decay = ADSRClipGeneratorPhaseData.Create(decay, allocator),
				Sustain = ADSRClipGeneratorPhaseData.Create(sustain, allocator),
				Release = ADSRClipGeneratorPhaseData.Create(release, allocator)
			};
		}

		public void Dispose()
		{
			Attack.Dispose();
			Decay.Dispose();
			Sustain.Dispose();
			Release.Dispose();
		}
	}
}