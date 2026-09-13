using System;
using Progression;
using Research;

namespace Peleng
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public abstract class PelengRequirement
	{
		public abstract bool Check(IProgressionState progression, IResearchProgress research);
		public abstract void Validate();
	}
}
