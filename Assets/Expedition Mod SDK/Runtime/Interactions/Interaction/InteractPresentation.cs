using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Search;

namespace Interactions.Interaction
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class InteractPresentation
	{
		[field: SerializeField] public LocalizedString Text { get; private set; } = new();
		[field: SerializeField, SearchContext("dir:Assets/UI/Cursors")] public Sprite Cursor { get; private set; }
		 private Sprite PreviewCursor => Cursor;
	}
}