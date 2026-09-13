using System;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class SolarBatteryConfig : ItemFeatureConfig
	{
		[field: SerializeField]
		public string SupplyId { get; private set; }
		[field: SerializeField, Min(0.01f)] public float ChargePerSecond { get; private set; } = 1f;

		protected override void ValidateConfiguration()
		{
			if (string.IsNullOrWhiteSpace(SupplyId) || !float.IsFinite(ChargePerSecond) || ChargePerSecond <= 0)
			{
				throw new InvalidOperationException("Solar battery requires supply and positive charging speed.");
			}
		}
	}
}
