using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Items
{
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
    public sealed class EquipmentConfig : ItemFeatureConfig, IEquipmentConfig
    {
        [field: SerializeField] public AssetReferenceGameObject Prefab { get; private set; } = new(null);

        [field: SerializeField]
        public string EquipmentSocket { get; private set; }

        public EquipmentConfig()
        {
        }

        private EquipmentConfig(AssetReferenceGameObject prefab)
        {
            Prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            Validate();
        }

        protected override void ValidateConfiguration()
        {
            if (Prefab == null || !Prefab.RuntimeKeyIsValid())
            {
                throw new InvalidOperationException($"{nameof(EquipmentConfig)} requires an equipment prefab.");
            }
        }
    }
}