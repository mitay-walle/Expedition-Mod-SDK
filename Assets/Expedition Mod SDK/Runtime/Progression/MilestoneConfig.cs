using System.Collections.Generic;
using Building;
using Items;
using Recipes;
using UnityEngine;
using UnityEngine.Localization;

namespace Progression
{
	[CreateAssetMenu(fileName = "Milestone", menuName = "Game/Progression/Milestone")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class MilestoneConfig : ScriptableObject, Peleng.IPelengMetadata
	{
		[SerializeField]
		private string _milestoneId;

		[SerializeField] private LocalizedString _title = new();
		[SerializeField] private LocalizedString _result = new();
		[SerializeField] private Sprite _icon;
		[SerializeField] private List<MilestoneConfig> _prerequisites = new();
		[SerializeField] private List<ItemConfig> _craftedOrPickedUpItems = new();
		[SerializeField] private List<RecipeConfig> _discoveredRecipes = new();
		[SerializeField] private List<BuildRecipeConfig> _builtRecipes = new();

		[SerializeField]
		private List<string> _progressionFlags = new();

		public LocalizedString PelengName => Title;
		public LocalizedString PelengDescription => Result;
		public Sprite PelengIcon => Icon;
		public UnityEngine.AddressableAssets.AssetReferenceSprite PelengIconReference => null;
		public string Id => _milestoneId;
		public LocalizedString Title => _title;
		public LocalizedString Result => _result;
		public Sprite Icon => _icon;
		public IReadOnlyList<MilestoneConfig> Prerequisites => _prerequisites;
		public IReadOnlyList<ItemConfig> CraftedOrPickedUpItems => _craftedOrPickedUpItems;
		public IReadOnlyList<RecipeConfig> DiscoveredRecipes => _discoveredRecipes;
		public IReadOnlyList<BuildRecipeConfig> BuiltRecipes => _builtRecipes;
		public IReadOnlyList<string> ProgressionFlags => _progressionFlags;

		public bool HasAutomaticRequirements =>
			_craftedOrPickedUpItems.Count > 0 || _discoveredRecipes.Count > 0 || _builtRecipes.Count > 0 ||
			_progressionFlags.Count > 0;
	}
}
