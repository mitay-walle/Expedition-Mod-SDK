using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Interaction
{
	[CreateAssetMenu(fileName = "Interact Presentation Catalog", menuName = "Expedition/Interact/Presentation Catalog")]
	
	[UnityEngine.Icon("Assets/Gizmos/InteractPresentation.png")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class InteractPresentationCatalog : ScriptableObject
	{
		[SerializeField]
		private Dictionary<string, InteractPresentation> _presentationById = new();

		public InteractPresentation Get(string id)
		{
			if (!_presentationById.TryGetValue(id, out InteractPresentation presentation))
			{
				throw new KeyNotFoundException($"Interaction presentation is not registered: '{id}'.");
			}

			return presentation;
		}

		public void Validate()
		{
			foreach ((string id, InteractPresentation presentation) in _presentationById)
			{
				if (string.IsNullOrWhiteSpace(id) || presentation == null ||
					presentation.Text == null || presentation.Text.IsEmpty || !presentation.Cursor)
				{
					throw new InvalidOperationException($"Interaction presentation catalog '{name}' contains an invalid entry.");
				}
			}
		}
	}
}
