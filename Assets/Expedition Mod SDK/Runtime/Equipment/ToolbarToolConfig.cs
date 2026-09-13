using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public class ToolbarToolConfig : ItemFeatureConfig, IEquipmentConfig
	{
		[field: SerializeField] public AssetReferenceGameObject Prefab { get; private set; } = new(null);
		public string EquipmentSocket => EquipmentUtility.RightHandSocket;

		protected override void ValidateConfiguration()
		{
			if (Prefab == null || !Prefab.RuntimeKeyIsValid())
			{
				throw new InvalidOperationException($"{nameof(ToolbarToolConfig)} requires an equipment prefab.");
			}
		}
	}
}
