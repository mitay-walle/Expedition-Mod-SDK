using System;
using UnityEngine;
using UnityEngine.Localization;
namespace Research
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class AtmosphereGas
	{
		public LocalizedString Name = new();
		[Range(0f, 100f)] public float Percentage;
	}
}
