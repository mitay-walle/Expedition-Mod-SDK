using System;
using UnityEngine;

namespace Building
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class BuildingSettings : ScriptableObject
	{
		[field: SerializeField] public Material HologramMaterial { get; private set; }
		[field: SerializeField] public Color ValidHologramColor { get; private set; } = new(0.2f, 1f, 0.45f, 0.55f);
		[field: SerializeField] public Color InvalidHologramColor { get; private set; } = new(1f, 0.2f, 0.2f, 0.55f);

		public void Validate()
		{
			if (!HologramMaterial)
			{
				throw new InvalidOperationException($"{nameof(BuildingSettings)} requires a hologram material.");
			}

			if (!HologramMaterial.HasProperty("_Color"))
			{
				throw new InvalidOperationException(
					$"Building hologram material '{HologramMaterial.name}' requires a _Color property.");
			}
		}
	}
}
