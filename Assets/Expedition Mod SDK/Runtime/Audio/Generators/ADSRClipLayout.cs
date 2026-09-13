using System;
using UnityEngine;

namespace Audio.Generators
{
	[Serializable]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public class ADSRClipLayout
	{
		[SerializeField] private ADSRClipSection _start = new ADSRClipSection();
		[SerializeField] private ADSRClipSection _loop = new ADSRClipSection();
		[SerializeField] private ADSRClipSection _end = new ADSRClipSection();

		public ADSRClipSection Start => _start;
		public ADSRClipSection Loop => _loop;
		public ADSRClipSection End => _end;
	}
}