using System;
using System.Collections.Generic;
using UnityEngine.Localization;
namespace Research
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public abstract class AnalyzerReadings
	{
		public abstract string TypeId { get; }
		public LocalizedString Conclusion = new();
		public abstract void Validate();
		public abstract IEnumerable<string> Describe();
		public abstract AnalyzerReadings Copy();
		protected static LocalizedString CopyText(LocalizedString text) => new(text.TableReference, text.TableEntryReference);
		protected static string Text(string key, params object[] args) => new LocalizedString("UI", key).GetLocalizedString(args);
	}
}
