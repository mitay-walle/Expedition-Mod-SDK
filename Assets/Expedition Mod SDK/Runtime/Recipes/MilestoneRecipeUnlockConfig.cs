using System.Collections.Generic;
using Progression;
using UnityEngine;

namespace Recipes
{
	[CreateAssetMenu(fileName = "Recipe Unlock", menuName = "Game/Recipes/Milestone Unlock")]
	
	[UnityEngine.Icon("Assets/Gizmos/Recipe.png")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class MilestoneRecipeUnlockConfig : ScriptableObject
	{
		[SerializeField] private MilestoneConfig _milestone;
		[SerializeField] private List<RecipeConfig> _recipes = new();

		public MilestoneConfig Milestone => _milestone;
		public IReadOnlyList<RecipeConfig> Recipes => _recipes;
	}
}
