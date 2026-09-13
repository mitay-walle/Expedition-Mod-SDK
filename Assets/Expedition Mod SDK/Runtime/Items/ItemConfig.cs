using UnityEngine.Audio;
using Game.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugging;
using mitaywalle.UI.Packages.GridImage.Runtime;
using Tools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Items
{
	[CreateAssetMenu(fileName = "Item", menuName = "Expedition/Items/Item Config")]
	
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class ItemConfig : ScriptableObject, IHierarchyError, Peleng.IPelengMetadata
	{
		[SerializeField]
		private string _itemId;

		[SerializeField] private LocalizedString _name = new();
		[SerializeField] private LocalizedString _description = new();

		[SerializeField]
		private string _category;

		[SerializeField] private List<IAudioGenerator.Serializable> _pickupAudio = new();
		[SerializeField] private IAudioGenerator.Serializable _useAudio;
		[SerializeField] private AssetReferenceGameObject _pickup = new(null);
		[SerializeField] private AssetReferenceSprite _sprite;
		[SerializeField] private GridShape _shape = GridShape.Default;
		[SerializeReference] private List<ItemFeatureConfig> _componentConfigs = new();

		public LocalizedString PelengName => Name;
		public LocalizedString PelengDescription => Description;
		public UnityEngine.Sprite PelengIcon => null;
		public AssetReferenceSprite PelengIconReference => Sprite;
		public string Id => _itemId;
		public LocalizedString Name => _name;
		public LocalizedString Description => _description;
		public string Category => _category;
		public AssetReferenceGameObject Pickup => _pickup;
		public AssetReferenceSprite Sprite => _sprite;
		public GridShape Shape => _shape;
		public IReadOnlyList<ItemFeatureConfig> FeatureConfigs => _componentConfigs;
		public IAudioGenerator UseAudio => _useAudio.definition;
		public IReadOnlyList<IAudioGenerator.Serializable> PickupAudio => _pickupAudio;

		public T GetFeature<T>() where T : class
		{
			return TryGetFeature(out T result) ? result : null;
		}

		public bool TryGetFeature<T>(out T result) where T : class
		{
			result = null;
			foreach (ItemFeatureConfig featureConfig in _componentConfigs)
			{
				if (featureConfig is T typedConfig)
				{
					if (result != null)
					{
						throw new InvalidOperationException(
							$"ItemConfig '{name}' contains more than one {typeof(T).Name} config.");
					}

					result = typedConfig;
				}
			}

			return result != null;
		}

		public IReadOnlyList<T> GetFeatures<T>() where T : class
		{
			var result = new List<T>();
			foreach (ItemFeatureConfig featureConfig in _componentConfigs)
			{
				if (featureConfig is T typedConfig)
				{
					result.Add(typedConfig);
				}
			}

			return result;
		}

		public string DisplayHierarchyError()
		{
			var errors = new List<string>();
			if (string.IsNullOrWhiteSpace(_itemId))
			{
				errors.Add("Item ID is required.");
			}

			if (_pickup == null || !_pickup.RuntimeKeyIsValid())
			{
				errors.Add("Pickup prefab is required.");
			}

			return string.Join("\n", errors);
		}

		public void Validate()
		{
			if (string.IsNullOrWhiteSpace(_itemId))
			{
				throw new InvalidOperationException($"ItemConfig '{name}' requires an id.");
			}

			if (string.IsNullOrWhiteSpace(_category))
			{
				throw new InvalidOperationException($"ItemConfig '{name}' requires a category.");
			}

			CreateFeatureConfigMap();
		}

		public Item Create()
		{
			CreateFeatureConfigMap();
			var components = new List<ItemComponent>();
			foreach (var featureConfig in _componentConfigs)
			{
				if (featureConfig is not IItemComponentConfig componentConfig)
				{
					continue;
				}

				ItemComponent component = componentConfig.CreateComponent();
				if (component == null || component.GetType() != componentConfig.ComponentType)
				{
					throw new InvalidOperationException(
						$"ItemConfig '{name}' feature config '{featureConfig.GetType().Name}' created an invalid runtime component.");
				}

				if (component is SupplySource { State: SupplySourceState.Source } supplySource &&
					string.IsNullOrEmpty(supplySource.ItemId))
				{
					supplySource.ItemId = Id;
				}

				componentConfig.ValidateComponent(component);
				components.Add(component);
			}

			return new Item(Id, components);
		}

		public Item Clone(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (!string.Equals(item.Config, Id, StringComparison.Ordinal))
			{
				throw new InvalidOperationException(
					$"Cannot clone item config {item.Config} through ItemConfig '{name}' ({Id}).");
			}

			var copy = JsonUtility.FromJson<Item>(JsonUtility.ToJson(item));
			Bind(copy);
			return copy;
		}

		private void Bind(Item item)
		{
			CreateFeatureConfigMap();
			var configsByComponentType = new Dictionary<Type, List<IItemComponentConfig>>();
			foreach (ItemFeatureConfig featureConfig in _componentConfigs)
			{
				if (featureConfig is not IItemComponentConfig componentConfig)
				{
					continue;
				}

				if (!configsByComponentType.TryGetValue(componentConfig.ComponentType, out List<IItemComponentConfig> configs))
				{
					configs = new List<IItemComponentConfig>();
					configsByComponentType.Add(componentConfig.ComponentType, configs);
				}

				configs.Add(componentConfig);
			}

			int expectedCount = configsByComponentType.Values.Sum(configs => configs.Count);
			if (item.Components.Count != expectedCount)
			{
				throw new InvalidOperationException(
					$"Item config {Id} contains {item.Components.Count} runtime components; expected {expectedCount}.");
			}

			var validatedCounts = new Dictionary<Type, int>();
			foreach (ItemComponent component in item.Components)
			{
				if (component == null)
				{
					throw new InvalidOperationException($"Item config {Id} contains an empty runtime component.");
				}

				Type componentType = component.GetType();
				if (!configsByComponentType.TryGetValue(componentType, out List<IItemComponentConfig> configs))
				{
					throw new InvalidOperationException(
						$"Item config {Id} contains unknown runtime component '{componentType.Name}'.");
				}

				validatedCounts.TryGetValue(componentType, out int validatedCount);
				if (validatedCount >= configs.Count)
				{
					throw new InvalidOperationException(
						$"Item config {Id} contains too many '{componentType.Name}' runtime components.");
				}

				configs[validatedCount].ValidateComponent(component);
				validatedCounts[componentType] = validatedCount + 1;
			}
		}

		private Dictionary<Type, ItemFeatureConfig> CreateFeatureConfigMap()
		{
			if (string.IsNullOrWhiteSpace(_itemId))
			{
				throw new InvalidOperationException($"ItemConfig '{name}' requires an id.");
			}

			var result = new Dictionary<Type, ItemFeatureConfig>();
			var useConfigCount = 0;
			foreach (var featureConfig in _componentConfigs)
			{
				if (featureConfig == null)
				{
					throw new InvalidOperationException($"ItemConfig '{name}' contains an empty feature config.");
				}

				featureConfig.Validate();
				var configType = featureConfig.GetType();
				if (!result.TryAdd(configType, featureConfig) && featureConfig is not IRepeatableItemComponentConfig)
				{
					throw new InvalidOperationException($"ItemConfig '{name}' contains duplicate feature config '{configType.Name}'.");
				}

				if (featureConfig is IItemComponentConfig componentConfig)
				{
					Type componentType = componentConfig.ComponentType;
					if (componentType == null || componentType.IsAbstract || !typeof(ItemComponent).IsAssignableFrom(componentType))
					{
						throw new InvalidOperationException(
							$"ItemConfig '{name}' feature config '{configType.Name}' declares an invalid runtime component type.");
					}
				}

				if (featureConfig is ItemUseConfig && ++useConfigCount > 1)
				{
					throw new InvalidOperationException($"ItemConfig '{name}' contains more than one use config.");
				}
			}

			IEquipmentConfig equipmentConfig = GetFeature<IEquipmentConfig>();
			if (result.ContainsKey(typeof(GatheringToolConfig)) && equipmentConfig == null)
			{
				throw new InvalidOperationException($"ItemConfig '{name}' with {nameof(GatheringToolConfig)} requires {nameof(IEquipmentConfig)}.");
			}

			if (result.TryGetValue(typeof(FlashlightConfig), out ItemFeatureConfig flashlightFeature))
			{
				var flashlight = (FlashlightConfig)flashlightFeature;
				if (equipmentConfig == null)
				{
					throw new InvalidOperationException(
						$"ItemConfig '{name}' with {nameof(FlashlightConfig)} requires {nameof(IEquipmentConfig)}.");
				}

				if (flashlight.UsesSupply && !GetFeatures<SupplySlotConfig>().Any(source =>
					    string.Equals(source.SupplyId, flashlight.SupplyId, StringComparison.Ordinal)))
				{
					throw new InvalidOperationException(
						$"ItemConfig '{name}' flashlight requires supply '{flashlight.SupplyId}' without a matching {nameof(SupplySlotConfig)}.");
				}

				if (!flashlight.UsesSupply && !result.ContainsKey(typeof(DurabilityConfig)))
				{
					throw new InvalidOperationException(
						$"ItemConfig '{name}' durability flashlight requires {nameof(DurabilityConfig)}.");
				}
			}

			if (result.TryGetValue(typeof(GatheringToolConfig), out ItemFeatureConfig gatheringFeature))
			{
				var supplyConfigs = GetFeatures<SupplySlotConfig>();
				foreach (GatheringToolMethod method in ((GatheringToolConfig)gatheringFeature).Methods)
				{
					if (method.SupplyCost > 0 && !supplyConfigs.Any(source =>
						    string.Equals(source.SupplyId, method.SupplyId, StringComparison.Ordinal)))
					{
						throw new InvalidOperationException(
							$"ItemConfig '{name}' gathering method requires supply '{method.SupplyId}' without a matching {nameof(SupplySourceConfig)}.");
					}
				}
			}

			if (result.TryGetValue(typeof(ScannerToolConfig), out ItemFeatureConfig scannerFeature))
			{
				ScannerToolConfig scanner = (ScannerToolConfig)scannerFeature;
				if (!GetFeatures<SupplySlotConfig>().Any(slot =>
					    string.Equals(slot.SupplyId, scanner.SupplyId, StringComparison.Ordinal)))
				{
					throw new InvalidOperationException(
						$"ItemConfig '{name}' scanner requires supply '{scanner.SupplyId}' without a matching {nameof(SupplySlotConfig)}.");
				}
			}

			return result;
		}
	}
}