using System;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class SupplySourceConfig : ItemFeatureConfig<SupplySource>, IRepeatableItemComponentConfig
	{
		[SerializeField]
		private string _supplyId;
		[SerializeField, Min(0)] private float _current;
		[SerializeField, Min(0)] private float _maximum;

		public string SupplyId => _supplyId;

		public SupplySourceConfig()
		{
		}

		public SupplySourceConfig(string supplyId, float current, float maximum)
		{
			_supplyId = supplyId;
			_current = current;
			_maximum = maximum;
			Validate();
		}

		protected override SupplySource CreateComponent() => new()
		{
			SupplyId = _supplyId,
			State = SupplySourceState.Source,
			Current = _current,
			Maximum = _maximum,
		};

		protected override void ValidateComponent(SupplySource component)
		{
			component.Validate();
			if (component.State != SupplySourceState.Source)
			{
				throw new InvalidOperationException($"{nameof(SupplySourceConfig)} requires a standalone runtime source.");
			}
		}

		protected override void ValidateConfiguration()
		{
			if (string.IsNullOrWhiteSpace(_supplyId) || _maximum <= 0 || _current < 0 || _current > _maximum)
			{
				throw new InvalidOperationException($"{nameof(SupplySourceConfig)} requires a supply id and charge in the range 0..Maximum.");
			}
		}
	}
}
