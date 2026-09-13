using System;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class DurabilityConfig : ItemFeatureConfig<Durability>
	{
		[field: SerializeField, Min(0.01f)] public float MaxDurability { get; private set; } = 100f;

		public DurabilityConfig()
		{
		}

		public DurabilityConfig(float maxDurability)
		{
			MaxDurability = maxDurability;
			Validate();
		}

		protected override Durability CreateComponent()
		{
			Validate();
			return new Durability(MaxDurability);
		}

		protected override void ValidateComponent(Durability durability)
		{
			Validate();
			if (durability.CurrentDurability < 0f || durability.CurrentDurability > MaxDurability)
			{
				throw new InvalidOperationException(
					$"{nameof(Durability)} value {durability.CurrentDurability} is outside 0..{MaxDurability}.");
			}
		}

		protected override void ValidateConfiguration()
		{
			if (MaxDurability <= 0f)
			{
				throw new InvalidOperationException($"{nameof(DurabilityConfig)} requires positive maximum durability.");
			}
		}
	}
}
