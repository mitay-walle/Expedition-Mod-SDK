using System;
using UnityEngine;

namespace Building
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class BuildCost
	{
		[SerializeField]
		private string _itemId;
		public string ItemId => _itemId;
		[field: SerializeField, Min(1)] public int Count { get; private set; } = 1;
	}
}
