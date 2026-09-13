using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	[CreateAssetMenu(menuName = "Game/UI/Input Prompt Catalog", fileName = "InputPromptCatalog")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class InputPromptCatalog : ScriptableObject
	{
		[SerializeField] private List<InputPromptEntry> _entries = new();

		public Sprite Get(InputPromptDeviceFamily deviceFamily, string controlPath)
		{
			if (string.IsNullOrWhiteSpace(controlPath))
			{
				return null;
			}

			foreach (InputPromptEntry entry in _entries)
			{
				if (entry.DeviceFamily == deviceFamily &&
				    string.Equals(entry.ControlPath, controlPath, StringComparison.OrdinalIgnoreCase))
				{
					return entry.Sprite;
				}
			}

			return null;
		}
	}
}