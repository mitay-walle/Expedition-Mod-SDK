using System;
using Progression;
using Research;
using UnityEngine;

namespace Peleng
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class CompletedMilestoneRequirement : PelengRequirement
	{
		[field: SerializeField] public MilestoneConfig Milestone { get; private set; }
		public override bool Check(IProgressionState progression, IResearchProgress research) => progression.IsCompleted(Milestone.Id);
		public override void Validate()
		{
			if (!Milestone)
			{
				throw new InvalidOperationException("Peleng requirement needs a milestone.");
			}
		}
	}
}
