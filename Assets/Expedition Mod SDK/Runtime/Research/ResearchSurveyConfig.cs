using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Research
{
	[CreateAssetMenu(menuName = "Expedition/Research/Survey")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class ResearchSurveyConfig : ResearchSampleConfig
	{
		[field: SerializeField] public List<AnalyzerZoneConfig> Zones { get; private set; } = new();
		public override bool RequiresAnalysis => false;
		public override bool CanSubmit(IResearchProgress session) => Zones.All(z => z.Requirements.All(r => session.IsAnalyzerReady(z.Id, r.TypeId)));
		public override bool AcceptsRecipient(string recipientId) => recipientId == "Scientist";
		public override string SubmitKey => "Analyzers.Report";
		public override void Validate()
		{
			base.Validate();
			if (Zones.Count == 0 || Zones.Any(z => !z) || Zones.Select(z => z.Id).Distinct(StringComparer.Ordinal).Count() != Zones.Count)
			{
				throw new InvalidOperationException("Survey requires unique zones.");
			}
			foreach (AnalyzerZoneConfig zone in Zones)
			{
				zone.Validate();
			}
		}
	}
}
