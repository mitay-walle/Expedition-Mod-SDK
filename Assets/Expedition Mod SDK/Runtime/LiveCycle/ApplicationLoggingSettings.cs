using System.Collections.Generic;
using UnityEngine;

namespace LiveCycle
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class ApplicationLoggingSettings : ScriptableObject
	{
		[field: SerializeField] public List<string> HiddenPrefixes { get; private set; } = new();
	}
}
