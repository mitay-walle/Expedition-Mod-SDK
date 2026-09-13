using System;
using Items;
using UnityEngine;

namespace Peleng
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class PelengToolConfig : ItemFeatureConfig
	{
		[field: SerializeField]
		public string SupplyId { get; private set; }
		[field: SerializeField, Min(0.01f)] public float CostPerSecond { get; private set; } = 1f;
		protected override void ValidateConfiguration()
		{
			if (string.IsNullOrWhiteSpace(SupplyId) || !float.IsFinite(CostPerSecond) || CostPerSecond <= 0)
			{
				throw new InvalidOperationException("Peleng tool needs supply and positive consumption.");
			}
		}
	}
}
