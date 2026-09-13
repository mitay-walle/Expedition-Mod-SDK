using UnityEngine.AddressableAssets;

namespace Items
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public interface IEquipmentConfig
	{
		AssetReferenceGameObject Prefab { get; }
		string EquipmentSocket { get; }
	}
}
