using System;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class ValueChangeConfig : ItemUseConfig
	{
		[field: SerializeField]
		public string Key { get; private set; } = "Health";
		[field: SerializeField] public ParticleSystem.MinMaxCurve Value { get; private set; } = new();
		[field: SerializeField]
		public string SupplyId { get; private set; }
		[field: SerializeField, Min(0)] public float SupplyCost { get; private set; }

		public ValueChangeConfig()
		{
		}

		public override bool IsConsumedOnUse => SupplyCost <= 0;

		public ValueChangeConfig(string key, ParticleSystem.MinMaxCurve value)
		{
			Key = key;
			Value = value;
			Validate();
		}

		protected override void ValidateConfiguration()
		{
			if (string.IsNullOrWhiteSpace(Key))
			{
				throw new InvalidOperationException($"{nameof(ValueChangeConfig)} requires an actor value key.");
			}

			if (SupplyCost > 0 && string.IsNullOrWhiteSpace(SupplyId))
			{
				throw new InvalidOperationException($"{nameof(ValueChangeConfig)} supply use requires a supply id.");
			}
		}
	}
}