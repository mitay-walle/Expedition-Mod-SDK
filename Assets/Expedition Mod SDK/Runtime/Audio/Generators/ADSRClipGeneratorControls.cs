using UnityEngine;
using UnityEngine.Audio;

namespace Audio.Generators
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public static class ADSRClipGeneratorControls
	{
		public static void NoteOn(AudioSource audioSource)
		{
			if (audioSource == null)
			{
				return;
			}

			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}

			Send(audioSource, ADSRClipGeneratorCommand.NoteOn);
		}

		public static void Release(AudioSource audioSource)
		{
			NoteOff(audioSource);
		}

		public static void NoteOff(AudioSource audioSource)
		{
			Send(audioSource, ADSRClipGeneratorCommand.NoteOff);
		}

		public static void Stop(AudioSource audioSource)
		{
			if (audioSource != null)
			{
				audioSource.Stop();
			}
		}

		public static void Pause(AudioSource audioSource)
		{
			if (audioSource != null)
			{
				audioSource.Pause();
			}
		}

		private static void Send(AudioSource audioSource, ADSRClipGeneratorCommand command)
		{
			if (audioSource == null)
			{
				return;
			}

			ProcessorInstance instance = audioSource.generatorInstance;
			if (!ControlContext.builtIn.Exists(instance))
			{
				return;
			}

			ADSRClipGeneratorMessage message = new ADSRClipGeneratorMessage(command);
			ControlContext.builtIn.SendMessage(instance, ref message);
		}
	}
}