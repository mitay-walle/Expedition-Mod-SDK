namespace Recipes
{
	public interface IRecipeDiscovery
	{
		bool IsDiscovered(string recipeId);
		bool TryDiscover(string recipeId);
	}
}
