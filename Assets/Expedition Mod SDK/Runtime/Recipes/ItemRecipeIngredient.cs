using System;
using Items;
using UnityEngine;

namespace Recipes
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class ItemRecipeIngredient
	{
		[SerializeField] private ItemConfig _item;
		[SerializeField, Min(1)] private int _count = 1;

		public ItemConfig Item => _item;
		public int Count => _count;
	}
}
