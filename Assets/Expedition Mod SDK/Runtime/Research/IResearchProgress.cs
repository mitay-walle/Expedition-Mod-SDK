namespace Research
{
	public interface IResearchProgress
	{
		bool IsCollected(string id);
		bool IsCompleted(string id);
		bool IsAnalyzerReady(string zoneId, string typeId);
	}
}
