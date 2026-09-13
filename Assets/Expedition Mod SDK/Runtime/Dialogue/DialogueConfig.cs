using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class DialogueConfig : ScriptableObject
	{
		[SerializeField, HideInInspector] private string _sourceGuid;
		[SerializeField] private IAudioGenerator.Serializable _audio;
		[SerializeField] private List<DialogueCaption> _captions = new();

		public string SourceGuid => _sourceGuid;
		public IAudioGenerator Audio => _audio.definition;
		public IReadOnlyList<DialogueCaption> Captions => _captions;
		[field: SerializeField] public DialogueEmotion Emotion { get; private set; }
		[field: SerializeField, Min(0.1f)] public float EmotionDuration { get; private set; } = 3f;

		public void SetImportedData(string sourceGuid, AudioClip audioClip, IReadOnlyList<DialogueCaption> captions)
		{
			if (string.IsNullOrWhiteSpace(sourceGuid))
			{
				throw new ArgumentException("Dialogue source GUID is required.", nameof(sourceGuid));
			}

			_sourceGuid = sourceGuid;
			_audio = new IAudioGenerator.Serializable(audioClip ? audioClip : throw new ArgumentNullException(nameof(audioClip)));
			_captions = captions != null ? new List<DialogueCaption>(captions) : throw new ArgumentNullException(nameof(captions));
		}
	}
}
