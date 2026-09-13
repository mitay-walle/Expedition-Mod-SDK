using System;
using Items;
using Interactions.Interaction;
using UnityEngine;
using System.Collections.Generic;

namespace Tools
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class GatheringToolConfig : ItemFeatureConfig
	{
		[SerializeField] private List<GatheringToolMethod> _methods = new();

		public IReadOnlyList<GatheringToolMethod> Methods => _methods;

		public GatheringToolConfig()
		{
		}

		public GatheringToolConfig(params GatheringToolMethod[] methods)
		{
			_methods = new List<GatheringToolMethod>(methods ?? throw new ArgumentNullException(nameof(methods)));
			Validate();
		}

		protected override void ValidateConfiguration()
		{
			if (_methods.Count == 0)
			{
				throw new InvalidOperationException($"{nameof(GatheringToolConfig)} requires at least one gathering method.");
			}

			foreach (GatheringToolMethod method in _methods)
			{
				if (method == null)
				{
					throw new InvalidOperationException($"{nameof(GatheringToolConfig)} contains an empty method.");
				}

				method.Validate();
			}
		}

		public bool TryGetAction(InteractActionConfig action, out GatheringToolMethod result)
		{
			foreach (GatheringToolMethod candidate in _methods)
			{
				if (candidate.Action == action)
				{
					result = candidate;
					return true;
				}
			}

			result = null;
			return false;
		}
	}
}
