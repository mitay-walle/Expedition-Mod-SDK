using System;
using Items;

namespace Research
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public abstract class ResearchToolConfig : ItemFeatureConfig
	{
		public abstract string ReadyMessage { get; }

		public abstract void Capture(IResearchCapture capture);
	}
}
