using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace Peleng
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public interface IPelengMetadata
	{
		LocalizedString PelengName { get; }
		LocalizedString PelengDescription { get; }
		Sprite PelengIcon { get; }
		AssetReferenceSprite PelengIconReference { get; }
	}
}
