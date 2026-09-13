using System;
using UnityEngine;

namespace Building
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class BuildingToolConfig : Items.ToolbarToolConfig
	{
		[field: SerializeField, Min(0.5f)] public float Range { get; private set; } = 4f;
	}
}
