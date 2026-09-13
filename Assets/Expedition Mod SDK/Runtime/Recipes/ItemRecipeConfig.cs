using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.Localization;

namespace Recipes
{
	[CreateAssetMenu(fileName = "Recipe", menuName = "Expedition/Recipes/Item Recipe")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class ItemRecipeConfig : RecipeConfig
	{
		[SerializeField] private ItemConfig _result;
		[SerializeField, Min(1)] private int _resultCount = 1;
		[SerializeField] private List<ItemRecipeIngredient> _ingredients = new();
		[SerializeField]
		private List<string> _craftingStationIds = new();
		[SerializeField, Min(0.1f)] private float _productionDuration = 1f;

		public override LocalizedString Name => _result ? _result.Name : null;
		public override Sprite Icon => null;
		public override string Category => _result ? _result.Category : null;
		public ItemConfig Result => _result;
		public int ResultCount => _resultCount;
		public IReadOnlyList<ItemRecipeIngredient> Ingredients => _ingredients;
		public IReadOnlyList<string> CraftingStationIds => _craftingStationIds;
		public float ProductionDuration => _productionDuration;

		public bool SupportsStation(string stationId)
		{
			if (string.IsNullOrWhiteSpace(stationId))
			{
				throw new ArgumentException("Crafting station id is required.", nameof(stationId));
			}

			foreach (string craftingStationId in _craftingStationIds)
			{
				if (string.Equals(craftingStationId, stationId, StringComparison.Ordinal))
				{
					return true;
				}
			}

			return false;
		}

		public override void Validate()
		{
			base.Validate();
			if (!_result || _resultCount <= 0)
			{
				throw new InvalidOperationException($"Item recipe '{name}' requires a result and positive result count.");
			}

			if (_ingredients.Count == 0)
			{
				throw new InvalidOperationException($"Item recipe '{name}' requires at least one ingredient.");
			}

			foreach (ItemRecipeIngredient ingredient in _ingredients)
			{
				if (ingredient == null || !ingredient.Item || ingredient.Count <= 0)
				{
					throw new InvalidOperationException($"Item recipe '{name}' contains an invalid ingredient.");
				}
			}

			if (_craftingStationIds.Count == 0 || _productionDuration <= 0f)
			{
				throw new InvalidOperationException($"Item recipe '{name}' requires a crafting station and production duration.");
			}

			var stationIds = new HashSet<string>(StringComparer.Ordinal);
			foreach (string stationId in _craftingStationIds)
			{
				if (string.IsNullOrWhiteSpace(stationId) || !stationIds.Add(stationId))
				{
					throw new InvalidOperationException($"Item recipe '{name}' contains an invalid crafting station id.");
				}
			}
		}
	}
}
