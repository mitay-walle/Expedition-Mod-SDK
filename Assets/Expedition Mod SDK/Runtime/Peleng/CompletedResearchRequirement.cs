using System;
using Progression;
using Research;
using UnityEngine;

namespace Peleng
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class CompletedResearchRequirement : PelengRequirement
	{
		[field: SerializeField] public ScriptableObject Research { get; private set; }

		public override bool Check(IProgressionState progression, IResearchProgress research) =>
			research.IsCompleted(((IResearchDefinition)Research).ResearchId);

		public override void Validate()
		{
			if (!Research || Research is not IResearchDefinition)
			{
				throw new InvalidOperationException("Peleng research requirement needs a research definition.");
			}
		}
	}
}
