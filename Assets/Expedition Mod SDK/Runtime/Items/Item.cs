using System;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class Item
	{
		[field: SerializeField] public string Config { get; private set; }
		[SerializeReference] private List<ItemComponent> _components = new();

		public IReadOnlyList<ItemComponent> Components => _components;

		public Item()
		{
		}

		public Item(string config, List<ItemComponent> components)
		{
			Config = string.IsNullOrWhiteSpace(config)
				? throw new ArgumentException("Item config id is required.", nameof(config))
				: config;
			_components = components ?? throw new ArgumentNullException(nameof(components));
		}

		public IReadOnlyList<T> GetComponents<T>() where T : ItemComponent
		{
			var result = new List<T>();
			foreach (ItemComponent component in _components)
			{
				if (component is T typedComponent)
				{
					result.Add(typedComponent);
				}
			}

			return result;
		}

		public T GetComponent<T>() where T : ItemComponent
		{
			T result = null;
			foreach (ItemComponent component in _components)
			{
				if (component is not T typedComponent)
				{
					continue;
				}

				if (result != null)
				{
					throw new InvalidOperationException($"Item '{Config}' contains more than one {typeof(T).Name} component.");
				}

				result = typedComponent;
			}

			return result;
		}

	}
}
