using System;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class Durability : ItemComponent
	{
		[field: SerializeField] public float CurrentDurability { get; private set; }

		public bool IsBroken => CurrentDurability <= 0f;

		public Durability()
		{
		}

		public Durability(float currentDurability)
		{
			if (currentDurability < 0f)
			{
				throw new ArgumentOutOfRangeException(nameof(currentDurability));
			}

			CurrentDurability = currentDurability;
		}

		public void Damage(float amount)
		{
			if (amount < 0f)
			{
				throw new ArgumentOutOfRangeException(nameof(amount));
			}

			CurrentDurability = Mathf.Max(0f, CurrentDurability - amount);
		}

		public void Repair(float amount, float maximumDurability)
		{
			if (amount < 0f)
			{
				throw new ArgumentOutOfRangeException(nameof(amount));
			}

			if (maximumDurability <= 0f || CurrentDurability > maximumDurability)
			{
				throw new ArgumentOutOfRangeException(nameof(maximumDurability));
			}

			CurrentDurability = Mathf.Min(maximumDurability, CurrentDurability + amount);
		}
	}
}
