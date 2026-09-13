namespace Audio.Generators
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public readonly struct ADSRClipGeneratorMessage
	{
		public readonly ADSRClipGeneratorCommand Command;

		public ADSRClipGeneratorMessage(ADSRClipGeneratorCommand command)
		{
			Command = command;
		}
	}
}