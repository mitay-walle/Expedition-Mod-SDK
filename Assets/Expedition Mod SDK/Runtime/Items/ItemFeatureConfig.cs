using System;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public abstract class ItemFeatureConfig
	{
		public void Validate()
		{
			ValidateConfiguration();
		}

		protected virtual void ValidateConfiguration()
		{
		}
	}
}
