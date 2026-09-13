using UnityEngine.Audio;
using Message = UnityEngine.Audio.ProcessorInstance.Message;
using Pipe = UnityEngine.Audio.ProcessorInstance.Pipe;
using Response = UnityEngine.Audio.ProcessorInstance.Response;

namespace Audio.Generators
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public struct ADSRClipGeneratorControl : GeneratorInstance.IControl<ADSRClipGeneratorRealtime>
	{
		public void Configure(ControlContext context, ref ADSRClipGeneratorRealtime realtime, in AudioFormat format, out GeneratorInstance.Setup setup, ref GeneratorInstance.Properties properties)
		{
			realtime.Configure(format.sampleRate);
			setup = new GeneratorInstance.Setup(format.speakerMode, format.sampleRate);
		}

		public void Dispose(ControlContext context, ref ADSRClipGeneratorRealtime realtime)
		{
			realtime.Dispose();
		}

		public void Update(ControlContext context, Pipe pipe)
		{
		}

		public Response OnMessage(ControlContext context, Pipe pipe, Message message)
		{
			if (!message.Is<ADSRClipGeneratorMessage>())
			{
				return Response.Unhandled;
			}

			ADSRClipGeneratorMessage generatorMessage = message.Get<ADSRClipGeneratorMessage>();
			pipe.SendData(context, generatorMessage);
			return Response.Handled;
		}
	}
}