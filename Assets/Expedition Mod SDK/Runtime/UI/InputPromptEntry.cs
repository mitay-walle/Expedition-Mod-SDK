using System;
using UnityEngine;

namespace UI
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class InputPromptEntry
	{
		[field: SerializeField] public InputPromptDeviceFamily DeviceFamily { get; private set; }
		[field: SerializeField] public string ControlPath { get; private set; }
		[field: SerializeField] public Sprite Sprite { get; private set; }
	}
}