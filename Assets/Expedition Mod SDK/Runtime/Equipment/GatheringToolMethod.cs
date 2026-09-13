using System;
using Interactions.Interaction;
using UnityEngine;

namespace Tools
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class GatheringToolMethod
	{
		[SerializeField] private InteractActionConfig _action;
		public InteractActionConfig Action => _action;
		[field: SerializeField, Min(0.01f)] public float Work { get; private set; } = 1;
		[field: SerializeField]
		public string SupplyId { get; private set; }
		[field: SerializeField, Min(0)] public float SupplyCost { get; private set; }

		public GatheringToolMethod()
		{
		}

		public GatheringToolMethod(InteractActionConfig action, float work, string supplyId = null, float supplyCost = 0)
		{
			_action = action;
			Work = work;
			SupplyId = supplyId;
			SupplyCost = supplyCost;
			Validate();
		}

		public void Validate()
		{
			if (!Action || Work <= 0)
			{
				throw new InvalidOperationException("A gathering tool method requires an action and positive work.");
			}

			if (SupplyCost > 0 && string.IsNullOrWhiteSpace(SupplyId))
			{
				throw new InvalidOperationException("A gathering supply cost requires a supply id.");
			}
		}
	}
}
