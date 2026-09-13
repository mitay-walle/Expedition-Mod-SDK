using Unity.Collections;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.Audio;
using CreationParameters = UnityEngine.Audio.ProcessorInstance.CreationParameters;

namespace Audio.Generators
{
	
	[CreateAssetMenu(fileName = "ADSRClipGenerator", menuName = "Audio/Generators/ADSR Clip Generator")]
	[UnityEngine.Icon("Assets/Scripts/Audio/Generators/Editor/Icons/ADSRClipGeneratorIcon.png")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public class ADSRClipGenerator : ScriptableObject, IAudioGenerator
	{
		[SerializeField] private ADSRClipLayout _clips = new ADSRClipLayout();
		[SerializeField] private ADSRPhase _attack = new ADSRPhase();
		[SerializeField] private ADSRPhase _decay = new ADSRPhase();
		[SerializeField] private ADSRPhase _sustain = new ADSRPhase();
		[SerializeField] private ADSRPhase _release = new ADSRPhase();

		[field: SerializeField, Min(0f)] public float PitchFactor { get; private set; } = 1f;
		[field: SerializeField, Min(0f)] public float VolumeFactor { get; private set; } = 1f;

		[field: SerializeField] public ADSRClipGeneratorRetriggerMode RetriggerMode { get; private set; } = ADSRClipGeneratorRetriggerMode.Restart;
		[field: SerializeField, Range(0f, 1f)] public float EnvelopeLerp { get; private set; } = 1f;

		public ADSRClipLayout Clips => _clips;
		public ADSRPhase Attack => _attack;
		public ADSRPhase Decay => _decay;
		public ADSRPhase Sustain => _sustain;
		public ADSRPhase Release => _release;

		public bool isFinite => false;
		public bool isRealtime => false;
		public DiscreteTime? length => null;

		public GeneratorInstance CreateInstance(ControlContext context, AudioFormat? nestedFormat, CreationParameters creationParameters)
		{
			ADSRClipGeneratorClipSet clipSet = ADSRClipGeneratorClipSet.Create(_clips, Allocator.Persistent);
			ADSRClipGeneratorPhaseSet phaseSet = ADSRClipGeneratorPhaseSet.Create(_attack, _decay, _sustain, _release, Allocator.Persistent);
			return context.AllocateGenerator(new ADSRClipGeneratorRealtime(clipSet, phaseSet, RetriggerMode, EnvelopeLerp, PitchFactor, VolumeFactor), new ADSRClipGeneratorControl(), nestedFormat,
				creationParameters);
		}
	}
}