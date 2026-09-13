using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Interaction
{
	[CreateAssetMenu(fileName = "Interact Action", menuName = "Expedition/Interact/Action")]
	
	[UnityEngine.Icon("Assets/Gizmos/InteractAction.png")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public class InteractActionConfig : ScriptableObject
	{
		[SerializeField]
		private string _presentationId;
		[SerializeField]
		private List<string> _blockerIds = new();
		[field: SerializeField] public InteractGesture Gesture { get; private set; } = new();
		public string PresentationId => _presentationId;
		public IReadOnlyList<string> Blockers => _blockerIds;

		public string GetPresentationId(string blocker)
		{
			if (string.IsNullOrEmpty(blocker))
			{
				return PresentationId;
			}

			if (_blockerIds.Contains(blocker))
			{
				return blocker;
			}

			throw new InvalidOperationException(
				$"Interaction action '{name}' has no presentation for blocker '{blocker}'.");
		}

		public void Validate()
		{
			if (string.IsNullOrWhiteSpace(PresentationId) || Gesture == null)
			{
				throw new InvalidOperationException($"Interaction action '{name}' requires a presentation id and gesture.");
			}

			var blockers = new HashSet<string>(StringComparer.Ordinal);
			foreach (string blocker in _blockerIds)
			{
				if (string.IsNullOrWhiteSpace(blocker) || !blockers.Add(blocker))
				{
					throw new InvalidOperationException($"Interaction action '{name}' contains duplicate blocker '{blocker}'.");
				}
			}

			Gesture.Validate();
		}
	}
}
