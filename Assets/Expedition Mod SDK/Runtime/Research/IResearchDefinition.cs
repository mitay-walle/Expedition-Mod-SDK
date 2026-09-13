using Items;
using Progression;
using Recipes;

namespace Research
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public interface IResearchDefinition
	{
		string ResearchId { get; }

		float Duration { get; }

		ItemConfig SourceItem { get; }

		bool IsCompleted(IProgressionState progression, IRecipeDiscovery recipes);
		bool IsUnlocked(IProgressionState progression);
		void Complete(IProgressionState progression, IRecipeDiscovery recipes);
	}
}
