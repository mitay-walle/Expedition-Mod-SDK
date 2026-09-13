using System;
using UnityEngine.Localization;

namespace Dialogue
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class DialogueCaption
	{
		public DialogueCaption(float entryTime, float exitTime, LocalizedString subtitle)
		{
			EntryTime = entryTime;
			ExitTime = exitTime;
			Subtitle = subtitle ?? throw new ArgumentNullException(nameof(subtitle));
		}

		[field: UnityEngine.SerializeField] public float EntryTime { get; private set; }
		[field: UnityEngine.SerializeField] public float ExitTime { get; private set; }
		[field: UnityEngine.SerializeField] public LocalizedString Subtitle { get; private set; } = new();
	}
}
