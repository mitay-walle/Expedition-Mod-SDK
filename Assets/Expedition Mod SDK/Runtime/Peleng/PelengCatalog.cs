using System.Collections.Generic;
using UnityEngine;

namespace Peleng
{
	[CreateAssetMenu(fileName = "PelengCatalog", menuName = "Expedition/Peleng/Catalog")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class PelengCatalog : ScriptableObject
	{
		[SerializeField] private List<PelengEntry> _entries = new();
		public IReadOnlyList<PelengEntry> Entries => _entries;
	}
}
