namespace Progression
{
	public interface IProgressionState
	{
		bool IsCompleted(string milestoneId);
		bool TryComplete(string milestoneId);
	}
}
