using Unity.Collections;

namespace Audio.Generators
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public struct ADSRClipGeneratorClipSet
	{
		public ADSRClipSectionSampleBuffer Start;
		public ADSRClipSectionSampleBuffer Loop;
		public ADSRClipSectionSampleBuffer End;

		public static ADSRClipGeneratorClipSet Create(ADSRClipLayout clips, Allocator allocator)
		{
			return new ADSRClipGeneratorClipSet
			{
				Start = ADSRClipSectionSampleBuffer.Create(clips?.Start, allocator),
				Loop = ADSRClipSectionSampleBuffer.Create(clips?.Loop, allocator),
				End = ADSRClipSectionSampleBuffer.Create(clips?.End, allocator)
			};
		}

		public void Dispose()
		{
			Start.Dispose();
			Loop.Dispose();
			End.Dispose();
		}
	}
}