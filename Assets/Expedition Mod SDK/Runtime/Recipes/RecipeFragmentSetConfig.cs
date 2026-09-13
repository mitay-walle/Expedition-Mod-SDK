using UnityEngine;
using UnityEngine.Localization;

namespace Recipes
{
	[CreateAssetMenu(fileName = "Recipe Fragment Set", menuName = "Game/Recipes/Fragment Set")]
	
	[UnityEngine.Icon("Assets/Gizmos/Recipe.png")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class RecipeFragmentSetConfig : ScriptableObject
	{
		[SerializeField]
		private string _fragmentSetId;
		[SerializeField] private RecipeConfig _recipe;
		[SerializeField, Min(1)] private int _requiredCount = 1;
		[SerializeField] private LocalizedString _title = new();
		[SerializeField] private Sprite _icon;

		public string Id => _fragmentSetId;
		public RecipeConfig Recipe => _recipe;
		public int RequiredCount => _requiredCount;
		public LocalizedString Title => _title;
		public Sprite Icon => _icon;
	}
}
