using System;

namespace Items
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public interface IItemComponentConfig
	{
		Type ComponentType { get; }
		ItemComponent CreateComponent();
		void ValidateComponent(ItemComponent component);
	}
}
