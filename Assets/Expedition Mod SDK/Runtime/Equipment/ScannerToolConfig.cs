using System;
using Interactions.Interaction;
using UnityEngine;

namespace Tools
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class ScannerToolConfig : Items.ItemFeatureConfig
	{
		[field: SerializeField] public InteractActionConfig Action { get; private set; }
		[field: SerializeField]
		public string SupplyId { get; private set; }
		[field: SerializeField, Min(0.01f)] public float SupplyCost { get; private set; } = 5f;

		protected override void ValidateConfiguration()
		{
			if (!Action || string.IsNullOrWhiteSpace(SupplyId) || SupplyCost <= 0f)
			{
				throw new InvalidOperationException(
					$"{nameof(ScannerToolConfig)} requires an action, supply id, and positive supply cost.");
			}

			Action.Validate();
		}
	}
}
