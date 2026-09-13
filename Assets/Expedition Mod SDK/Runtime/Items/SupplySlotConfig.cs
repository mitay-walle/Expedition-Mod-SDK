using System;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class SupplySlotConfig : ItemFeatureConfig<SupplySource>, IRepeatableItemComponentConfig
	{
		[SerializeField]
		private string _supplyId;

		public string SupplyId => _supplyId;

		public SupplySlotConfig()
		{
		}

		public SupplySlotConfig(string supplyId)
		{
			_supplyId = supplyId;
			Validate();
		}

		protected override SupplySource CreateComponent() => new()
		{
			SupplyId = _supplyId,
			State = SupplySourceState.EmptySlot,
		};

		protected override void ValidateComponent(SupplySource component)
		{
			component.Validate();
			if (component.State == SupplySourceState.Source || component.SupplyId != _supplyId)
			{
				throw new InvalidOperationException($"{nameof(SupplySlotConfig)} requires an empty or installed runtime slot with the configured supply id.");
			}
		}

		protected override void ValidateConfiguration() => CreateComponent().Validate();
	}
}
