using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Recipes
{
	
	[UnityEngine.Icon("Assets/Gizmos/Recipe.png")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public abstract class RecipeConfig : ScriptableObject
	{
		[SerializeField]
		private string _recipeId;
		public string Id => _recipeId;
		public abstract string Category { get; }
		public abstract LocalizedString Name { get; }
		public abstract Sprite Icon { get; }

		public virtual void Validate()
		{
			if (string.IsNullOrWhiteSpace(_recipeId))
			{
				throw new InvalidOperationException($"Recipe '{name}' requires an id.");
			}
		}
	}
}
