using System;
using UnityEngine;
namespace Research
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class AnalyzerToolConfig : ResearchToolConfig
	{
		 public string TypeId;
		 public string PlacedPrefabId;
		[Min(0.1f)] public float Range = 3f;
		public Vector3 HalfExtents = new(0.25f, 0.35f, 0.25f);
		public override string ReadyMessage => "Analyzers.Place";
		public override void Capture(IResearchCapture capture) => capture.PlaceAnalyzer(this);
		protected override void ValidateConfiguration()
		{
			if (string.IsNullOrWhiteSpace(TypeId) || string.IsNullOrWhiteSpace(PlacedPrefabId) || Range <= 0 || HalfExtents.x <= 0 || HalfExtents.y <= 0 || HalfExtents.z <= 0)
			{
				throw new InvalidOperationException("Invalid analyzer tool.");
			}
		}
	}
}
