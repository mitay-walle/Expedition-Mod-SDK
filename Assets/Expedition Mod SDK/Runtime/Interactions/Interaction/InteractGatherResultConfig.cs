using UnityEngine.Audio;
using System;
using Tools;
using UnityEngine;

namespace Interactions.Interaction
{
	[CreateAssetMenu(fileName = "Interact Gather Result", menuName = "Expedition/Interact/Gather Result")]
	
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class InteractGatherResultConfig : ScriptableObject
	{
		
		[SerializeField] private InteractActionConfig _action;
		[SerializeField] private InteractSourceKind _sourceKind;
		[SerializeField, Min(0.01f)] private float _bareHandWork = 1;
		public InteractActionConfig Action => _action;
		public InteractSourceKind SourceKind => _sourceKind;
		public float BareHandWork => _bareHandWork;
		[field: SerializeField] public int Priority { get; private set; }

		[field: SerializeField] public ParticleSystem ProcessVfx { get; private set; }
		[field: SerializeField] public IAudioGenerator.Serializable ProcessLoopAudio { get; private set; }
		[field: SerializeField] public ParticleSystem ProgressVfx { get; private set; }
		[field: SerializeField] public IAudioGenerator.Serializable ProgressSfxAudio { get; private set; }
		[field: SerializeField] public ParticleSystem ConfirmedToolVfx { get; private set; }
		[field: SerializeField] public IAudioGenerator.Serializable ConfirmedToolSfxAudio { get; private set; }
		[field: SerializeField] public ParticleSystem ConfirmedResourceVfx { get; private set; }
		[field: SerializeField] public IAudioGenerator.Serializable ConfirmedResourceSfxAudio { get; private set; }
		[field: SerializeField] public IAudioGenerator.Serializable RejectedSfxAudio { get; private set; }
		[field: SerializeField] public IAudioGenerator.Serializable CancelSfxAudio { get; private set; }
		[field: SerializeField] public ParticleSystem CompleteVfx { get; private set; }
		[field: SerializeField] public IAudioGenerator.Serializable CompleteSfxAudio { get; private set; }

		public void Validate()
		{
			if (!Action)
			{
				throw new InvalidOperationException($"Gather result '{name}' requires an interaction action.");
			}

			if (SourceKind == InteractSourceKind.BareHands && BareHandWork <= 0)
			{
				throw new InvalidOperationException($"Bare-hand gather result '{name}' requires positive work.");
			}

			Action.Validate();
		}
	}
}
