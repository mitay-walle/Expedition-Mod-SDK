using System;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class SupplySource : ItemComponent
	{
		
		public string SupplyId;
		
		public string ItemId;
		public SupplySourceState State;
		[Min(0)] public float Current;
		[Min(0)] public float Maximum;

		public bool HasSource => State != SupplySourceState.EmptySlot;

		public void Validate()
		{
			if (string.IsNullOrWhiteSpace(SupplyId))
			{
				throw new InvalidOperationException("A supply source requires a supply id.");
			}

			if (State == SupplySourceState.EmptySlot)
			{
				if (!string.IsNullOrEmpty(ItemId) || Current != 0 || Maximum != 0)
				{
					throw new InvalidOperationException("An empty supply slot cannot contain an item or charge.");
				}

				return;
			}

			if (string.IsNullOrWhiteSpace(ItemId) || Maximum <= 0 || Current < 0 || Current > Maximum)
			{
				throw new InvalidOperationException("A sourced supply requires ids and charge in the range 0..Maximum.");
			}
		}
	}
}
