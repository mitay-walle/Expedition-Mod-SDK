using System;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public abstract class ItemUseConfig : ItemFeatureConfig
	{
		[field: SerializeField] public ItemUseTarget Target { get; private set; }
		public abstract bool IsConsumedOnUse { get; }
	}
}
