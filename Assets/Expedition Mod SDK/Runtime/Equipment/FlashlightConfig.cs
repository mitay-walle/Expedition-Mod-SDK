using System;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class FlashlightConfig : ItemFeatureConfig
	{
		[SerializeField]
		private string _supplyId;
		[field: SerializeField, Min(0f)] public float SupplyCostPerSecond { get; private set; }
		[field: SerializeField, Min(0f)] public float DurabilityCostPerSecond { get; private set; }

		public string SupplyId => _supplyId;
		public bool UsesSupply => SupplyCostPerSecond > 0f;

		protected override void ValidateConfiguration()
		{
			bool usesSupply = SupplyCostPerSecond > 0f;
			bool usesDurability = DurabilityCostPerSecond > 0f;
			if (usesSupply == usesDurability)
			{
				throw new InvalidOperationException(
					$"{nameof(FlashlightConfig)} requires exactly one positive consumption rate.");
			}

			if (usesSupply && string.IsNullOrWhiteSpace(_supplyId))
			{
				throw new InvalidOperationException($"{nameof(FlashlightConfig)} supply use requires a supply id.");
			}
		}
	}
}
