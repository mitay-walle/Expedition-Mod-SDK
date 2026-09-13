using System;

namespace Items
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public abstract class ItemFeatureConfig<TComponent> : ItemFeatureConfig, IItemComponentConfig
		where TComponent : ItemComponent
	{
		public Type ComponentType => typeof(TComponent);

		ItemComponent IItemComponentConfig.CreateComponent()
		{
			return CreateComponent();
		}

		void IItemComponentConfig.ValidateComponent(ItemComponent component)
		{
			if (component is not TComponent typedComponent)
			{
				throw new InvalidOperationException($"{GetType().Name} expected {typeof(TComponent).Name} component.");
			}

			ValidateComponent(typedComponent);
		}

		protected abstract TComponent CreateComponent();

		protected abstract void ValidateComponent(TComponent component);
	}
}
