using System;
using System.Collections.Generic;
using Recipes;
using UnityEngine;
using UnityEngine.Localization;

namespace Building
{
	[CreateAssetMenu(fileName = "Recipe", menuName = "Game/Building/Recipe")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class BuildRecipeConfig : RecipeConfig
	{
		[SerializeField] private LocalizedString _name = new();
		[SerializeField] private Sprite _icon;
		[SerializeField]
		private string _category;
		[SerializeField]
		private string _persistentPrefabId;
		[SerializeField] private BuildPlacementType _placementType;
		[SerializeField]
		private string _snapType;
		[SerializeField, Min(1f)] private float _rotationStep = 15f;
		[SerializeField, Range(0f, 89f)] private float _maximumSlope = 35f;
		[SerializeField] private List<BuildCost> _costs = new();

		public override LocalizedString Name => _name;
		public override Sprite Icon => _icon;
		public override string Category => _category;
		public string PersistentPrefabId => _persistentPrefabId;
		public BuildPlacementType PlacementType => _placementType;
		public string SnapType => _snapType;
		public float RotationStep => _rotationStep;
		public float MaximumSlope => _maximumSlope;
		public IReadOnlyList<BuildCost> Costs => _costs;

		public int TotalMaterialCount
		{
			get
			{
				int result = 0;
				foreach (BuildCost cost in _costs)
				{
					result += cost.Count;
				}

				return result;
			}
		}


		public override void Validate()
		{
			base.Validate();
			if (string.IsNullOrWhiteSpace(_category))
			{
				throw new InvalidOperationException($"Build recipe '{name}' requires a category.");
			}

			if (string.IsNullOrWhiteSpace(_persistentPrefabId))
			{
				throw new InvalidOperationException($"Build recipe '{name}' is incomplete.");
			}

			if (_costs.Count == 0 || TotalMaterialCount <= 0)
			{
				throw new InvalidOperationException($"Build recipe '{name}' requires at least one material.");
			}

			if (_placementType == BuildPlacementType.Snap && string.IsNullOrWhiteSpace(_snapType))
			{
				throw new InvalidOperationException($"Snap recipe '{name}' requires a snap type.");
			}
		}
	}
}
