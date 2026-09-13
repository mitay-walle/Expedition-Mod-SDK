using Unity.Collections;
using UnityEngine;

namespace Audio.Generators
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public struct ADSRCurveTable
	{
		public const int Resolution = 128;

		public NativeArray<float> Values;

		public bool HasValues => Values.IsCreated && Values.Length > 0;

		public static ADSRCurveTable Create(AnimationCurve curve, Allocator allocator)
		{
			NativeArray<float> values = new NativeArray<float>(Resolution, allocator, NativeArrayOptions.UninitializedMemory);
			AnimationCurve evaluatedCurve = curve ?? AnimationCurve.Constant(0f, 1f, 1f);
			for (int index = 0; index < values.Length; index++)
			{
				float time = values.Length <= 1 ? 0f : index / (float)(values.Length - 1);
				values[index] = evaluatedCurve.Evaluate(time);
			}

			return new ADSRCurveTable { Values = values };
		}

		public float Evaluate(float normalizedTime)
		{
			if (!HasValues)
			{
				return 1f;
			}

			float clampedTime = normalizedTime < 0f ? 0f : normalizedTime > 1f ? 1f : normalizedTime;
			float position = clampedTime * (Values.Length - 1);
			int firstIndex = (int)position;
			int secondIndex = firstIndex + 1 < Values.Length ? firstIndex + 1 : firstIndex;
			float transition = position - firstIndex;
			float firstValue = Values[firstIndex];
			return firstValue + (Values[secondIndex] - firstValue) * transition;
		}

		public void Dispose()
		{
			if (Values.IsCreated)
			{
				Values.Dispose();
			}
		}
	}
}