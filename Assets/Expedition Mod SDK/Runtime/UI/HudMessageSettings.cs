using System;
using UnityEngine;

namespace UI
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public sealed class HudMessageSettings : ScriptableObject
	{
		[SerializeField] private HudMessageSeverity _minimumSeverity = HudMessageSeverity.Info;
		[SerializeField] private Color _infoColor = new(0.91f, 0.97f, 0.95f);
		[SerializeField] private Color _warningColor = new(0.96f, 0.87f, 0.41f);
		[SerializeField] private Color _errorColor = new(1f, 0.48f, 0.4f);

		public bool Includes(HudMessageSeverity severity)
		{
			return _minimumSeverity != HudMessageSeverity.Off && severity >= _minimumSeverity;
		}

		public Color GetColor(HudMessageSeverity severity)
		{
			return severity switch
			{
				HudMessageSeverity.Info => _infoColor,
				HudMessageSeverity.Warning => _warningColor,
				HudMessageSeverity.Error => _errorColor,
				_ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null),
			};
		}
	}
}
