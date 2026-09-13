using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
namespace Research
{
	[CreateAssetMenu(menuName = "Expedition/Research/Analyzer Zone")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class AnalyzerZoneConfig : ScriptableObject
	{
		[field: SerializeField] public string Id { get; private set; }
		[field: SerializeField] public LocalizedString Title { get; private set; }
		[field: SerializeField] public List<AnalyzerRequirement> Requirements { get; private set; } = new();
		public void Validate()
		{
			if (string.IsNullOrWhiteSpace(Id) || Title.IsEmpty || Requirements.Count == 0 || Requirements.Any(r => r == null) || Requirements.Select(r => r.TypeId).Distinct(StringComparer.Ordinal).Count() != Requirements.Count)
			{
				throw new InvalidOperationException("Invalid analyzer zone or duplicate analyzer type.");
			}
			foreach (AnalyzerRequirement requirement in Requirements)
			{
				requirement.Validate();
			}
		}
	}
}
