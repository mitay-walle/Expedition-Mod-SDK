using Unity.Collections;
using UnityEngine;

namespace Audio.Generators
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public struct ADSRClipSectionSampleBuffer
	{
		public NativeArray<float> Samples;
		public int Channels;
		public int SourceFrameCount;
		public int BeginFrame;
		public int EndFrame;
		public int Frequency;

		public int FrameCount => EndFrame > BeginFrame ? EndFrame - BeginFrame : 0;
		public bool HasSamples => Samples.IsCreated && Samples.Length > 0 && Channels > 0 && FrameCount > 0 && Frequency > 0;

		public static ADSRClipSectionSampleBuffer Create(ADSRClipSection section, Allocator allocator)
		{
			AudioClip clip = section?.Clip;
			if (clip == null)
			{
				return default;
			}

			int sampleCount = clip.samples * clip.channels;
			float[] managedSamples = new float[sampleCount];
			if (!clip.GetData(managedSamples, 0))
			{
				Debug.LogError($"[ADSRClipGenerator] Could not read PCM data from clip '{clip.name}'. Enable a readable/decompressed import mode for ADSR generator clips.");
				return default;
			}

			NativeArray<float> samples = new NativeArray<float>(managedSamples.Length, allocator, NativeArrayOptions.UninitializedMemory);
			samples.CopyFrom(managedSamples);

			float clipLength = clip.length;
			float startTime = Mathf.Clamp(section.StartTime, 0f, clipLength);
			float endTime = section.EndTime > section.StartTime ? Mathf.Clamp(section.EndTime, startTime, clipLength) : clipLength;
			int beginFrame = Mathf.Clamp(Mathf.FloorToInt(startTime * clip.frequency), 0, clip.samples);
			int endFrame = Mathf.Clamp(Mathf.CeilToInt(endTime * clip.frequency), beginFrame, clip.samples);

			return new ADSRClipSectionSampleBuffer
			{
				Samples = samples,
				Channels = clip.channels,
				SourceFrameCount = clip.samples,
				BeginFrame = beginFrame,
				EndFrame = endFrame,
				Frequency = clip.frequency
			};
		}

		public void Dispose()
		{
			if (Samples.IsCreated)
			{
				Samples.Dispose();
			}
		}
	}
}