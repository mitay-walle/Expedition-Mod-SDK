using System;
using Game.Audio;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class FootwearConfig : ItemFeatureConfig
	{
		[field: SerializeField] public FootstepAudioProfile FootstepAudio { get; private set; }
		[field: SerializeField, Min(0.01f)] public float MovementSpeedFactor { get; private set; } = 1f;
		[field: SerializeField, Min(0.01f)] public float SprintSpeedFactor { get; private set; } = 1f;
		[field: SerializeField, Min(0.01f)] public float JumpHeightFactor { get; private set; } = 1f;
		[field: SerializeField, Min(0f)] public float DurabilityCostPerSecond { get; private set; }
		[field: SerializeField, Min(0f)] public float SupplyCostPerSecond { get; private set; }
		[field: SerializeField, Min(0f)] public float SupplyCostPerJump { get; private set; }
		[field: SerializeField]
		public string SupplyId { get; private set; }

		public FootwearConfig()
		{
		}

		public FootwearConfig(
			float movementSpeedFactor,
			float sprintSpeedFactor,
			float jumpHeightFactor,
			float durabilityCostPerSecond = 0f,
			string supplyId = null,
			float supplyCostPerSecond = 0f,
			float supplyCostPerJump = 0f)
		{
			MovementSpeedFactor = movementSpeedFactor;
			SprintSpeedFactor = sprintSpeedFactor;
			JumpHeightFactor = jumpHeightFactor;
			DurabilityCostPerSecond = durabilityCostPerSecond;
			SupplyId = supplyId;
			SupplyCostPerSecond = supplyCostPerSecond;
			SupplyCostPerJump = supplyCostPerJump;
			Validate();
		}

		protected override void ValidateConfiguration()
		{
			if (MovementSpeedFactor <= 0f || SprintSpeedFactor <= 0f || JumpHeightFactor <= 0f)
			{
				throw new InvalidOperationException($"{nameof(FootwearConfig)} factors must be positive.");
			}

			if (DurabilityCostPerSecond < 0f || SupplyCostPerSecond < 0f || SupplyCostPerJump < 0f)
			{
				throw new InvalidOperationException($"{nameof(FootwearConfig)} costs cannot be negative.");
			}

			if ((SupplyCostPerSecond > 0f || SupplyCostPerJump > 0f) && string.IsNullOrWhiteSpace(SupplyId))
			{
				throw new InvalidOperationException($"{nameof(FootwearConfig)} requires a supply id for powered effects.");
			}
		}
	}
}
