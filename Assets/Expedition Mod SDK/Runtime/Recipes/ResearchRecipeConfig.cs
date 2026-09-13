using System;
using Items;
using UnityEngine;

namespace Recipes
{
	[CreateAssetMenu(fileName = "Research Recipe", menuName = "Expedition/Recipes/Research Recipe")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class ResearchRecipeConfig : ScriptableObject, Research.IResearchDefinition
	{
		[field: SerializeField] public ItemConfig SourceItem { get; private set; }
		[field: SerializeField] public RecipeConfig Result { get; private set; }
		[field: SerializeField, Min(1f)] public float Duration { get; private set; } = 30f;

		public string ResearchId => "Recipe/" + Result.Id;
		public bool IsCompleted(Progression.IProgressionState progression, IRecipeDiscovery recipes) => recipes.IsDiscovered(Result.Id);
		public bool IsUnlocked(Progression.IProgressionState progression) => true;
		public void Complete(Progression.IProgressionState progression, IRecipeDiscovery recipes)
		{
			recipes.TryDiscover(Result.Id);
		}

		public void Validate()
		{
			if (!SourceItem || !Result || Duration <= 0f)
			{
				throw new InvalidOperationException($"Research recipe '{name}' requires a source item, result recipe, and positive duration.");
			}
		}
	}
}
