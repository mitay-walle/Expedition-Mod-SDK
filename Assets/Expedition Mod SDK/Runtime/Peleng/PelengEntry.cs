using System;
using System.Collections.Generic;
using Progression;
using Research;
using UnityEngine;

namespace Peleng
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class PelengEntry
	{
		[field: SerializeField]
		public string Id { get; private set; }
		[field: SerializeField] public ScriptableObject Source { get; private set; }
		[field: SerializeField, Min(0.1f)] public float Range { get; private set; } = 100f;
		[SerializeReference] private List<PelengRequirement> _requirements = new();
		public IPelengMetadata Metadata => (IPelengMetadata)Source;

		public bool IsAvailable(IProgressionState progression, IResearchProgress research)
		{
			foreach (PelengRequirement requirement in _requirements)
			{
				if (!requirement.Check(progression, research))
				{
					return false;
				}
			}
			return true;
		}

		public void Validate()
		{
			if (string.IsNullOrWhiteSpace(Id) || !Source || Source is not IPelengMetadata || !float.IsFinite(Range) || Range <= 0)
			{
				throw new InvalidOperationException($"Invalid peleng entry '{Id}': source metadata and positive finite range are required.");
			}
			foreach (PelengRequirement requirement in _requirements)
			{
				if (requirement == null)
				{
					throw new InvalidOperationException($"Peleng entry '{Id}' contains an empty requirement.");
				}
				requirement.Validate();
			}
		}
	}
}
