using System;
using System.Collections.Generic;
using Items;
using Tools;
using UnityEngine;

namespace Interactions.Interaction
{
	[CreateAssetMenu(fileName = "Interact Gather Source", menuName = "Expedition/Interact/Gather Source")]
	
	[UnityEngine.Icon("Assets/Gizmos/InteractGatherSource.png")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class InteractGatherSourceConfig : ScriptableObject
	{
		[field: SerializeField] public ItemConfig Reward { get; private set; }
		[field: SerializeField, Min(1)] public int TotalUnits { get; private set; } = 1;
		[field: SerializeField, Min(1)] public int UnitsPerExtraction { get; private set; } = 1;
		[field: SerializeField, Min(0.01f)] public float WorkPerExtraction { get; private set; } = 1;
		[SerializeField] private List<InteractGatherResultConfig> _results = new();

		public IReadOnlyList<InteractGatherResultConfig> Results => _results;

		public int GetExtractionCount(int remainingUnits)
		{
			if (remainingUnits < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(remainingUnits));
			}

			return Mathf.Min(UnitsPerExtraction, remainingUnits);
		}

		public float GetProgress(int remainingUnits, float currentWork)
		{
			float partialUnits = Mathf.Clamp01(currentWork / WorkPerExtraction) * GetExtractionCount(remainingUnits);
			return Mathf.Clamp01((TotalUnits - remainingUnits + partialUnits) / TotalUnits);
		}

		public void Validate()
		{
			if (!Reward || TotalUnits <= 0 || UnitsPerExtraction <= 0 || WorkPerExtraction <= 0 ||
				_results.Count == 0)
			{
				throw new InvalidOperationException($"Gather source '{name}' is incomplete.");
			}

			var actions = new HashSet<InteractActionConfig>();
			foreach (InteractGatherResultConfig result in _results)
			{
				if (!result)
				{
					throw new InvalidOperationException($"Gather source '{name}' contains an empty result.");
				}

				result.Validate();
				if (!actions.Add(result.Action))
				{
					throw new InvalidOperationException($"Gather source '{name}' contains duplicate action '{result.Action.name}'.");
				}
			}
		}
	}
}
