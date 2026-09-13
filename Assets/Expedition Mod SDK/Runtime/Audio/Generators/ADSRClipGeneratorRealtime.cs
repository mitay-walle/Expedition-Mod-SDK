using System;
using Unity.IntegerTime;
using UnityEngine.Audio;
using AvailableData = UnityEngine.Audio.ProcessorInstance.AvailableData;
using Pipe = UnityEngine.Audio.ProcessorInstance.Pipe;
using UpdatedDataContext = UnityEngine.Audio.ProcessorInstance.UpdatedDataContext;

namespace Audio.Generators
{
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public struct ADSRClipGeneratorRealtime : GeneratorInstance.IRealtime
	{
		private ADSRClipGeneratorClipSet _clips;
		private ADSRClipGeneratorPhaseSet _phases;
		private ADSRClipGeneratorSegment _segment;
		private ADSRClipGeneratorEnvelopeStage _stage;
		private double _segmentPosition;
		private float _stageTime;
		private float _sampleRate;
		private float _currentVolume;
		private float _currentPitch;
		private float _releaseVolumeBase;
		private float _releasePitchBase;
		private ADSRClipGeneratorRetriggerMode _retriggerMode;
		private float _envelopeLerp;
		private readonly float _pitchFactor;
		private readonly float _volumeFactor;

		public ADSRClipGeneratorRealtime(ADSRClipGeneratorClipSet clips,
		                                 ADSRClipGeneratorPhaseSet phases,
		                                 ADSRClipGeneratorRetriggerMode retriggerMode,
		                                 float envelopeLerp,
		                                 float pitchFactor,
		                                 float volumeFactor)
		{
			_clips = clips;
			_phases = phases;
			_segment = ADSRClipGeneratorSegment.Silence;
			_stage = ADSRClipGeneratorEnvelopeStage.Idle;
			_segmentPosition = 0d;
			_stageTime = 0f;
			_sampleRate = 44100f;
			_currentVolume = 0f;
			_currentPitch = 1f;
			_releaseVolumeBase = 0f;
			_releasePitchBase = 1f;
			_retriggerMode = retriggerMode;
			_envelopeLerp = Clamp01(envelopeLerp);
			_pitchFactor = Math.Max(0f, pitchFactor);
			_volumeFactor = Math.Max(0f, volumeFactor);
		}

		public bool isFinite => false;
		public bool isRealtime => false;
		public DiscreteTime? length => null;

		public void Update(UpdatedDataContext context, Pipe pipe)
		{
			foreach (AvailableData.Element data in pipe.GetAvailableData(context))
			{
				if (data.TryGetData(out ADSRClipGeneratorMessage message))
				{
					Apply(message);
				}
			}
		}

		public GeneratorInstance.Result Process(in RealtimeContext context, Pipe pipe, ChannelBuffer buffer, GeneratorInstance.Arguments arguments)
		{
			if (_stage == ADSRClipGeneratorEnvelopeStage.Idle)
			{
				buffer.Clear();
				return buffer.frameCount;
			}

			if (_stage == ADSRClipGeneratorEnvelopeStage.Finished && _segment == ADSRClipGeneratorSegment.Finished)
			{
				buffer.Clear();
				return GeneratorInstance.Result.Finished(buffer.frameCount);
			}

			float sampleDelta = _sampleRate > 0f ? 1f / _sampleRate : 0f;
			for (int frame = 0; frame < buffer.frameCount; frame++)
			{
				UpdateEnvelope(sampleDelta);

				if (_stage == ADSRClipGeneratorEnvelopeStage.Finished && (_segment == ADSRClipGeneratorSegment.Finished || !_clips.End.HasSamples))
				{
					ClearFrom(buffer, frame);
					return GeneratorInstance.Result.Finished(buffer.frameCount);
				}

				for (int channel = 0; channel < buffer.channelCount; channel++)
				{
					buffer[channel, frame] = ReadCurrentSample(channel) * _currentVolume * _volumeFactor;
				}

				AdvanceSegment();
			}

			return buffer.frameCount;
		}

		public void Configure(float sampleRate)
		{
			_sampleRate = sampleRate > 0f ? sampleRate : 44100f;
		}

		public void Dispose()
		{
			_clips.Dispose();
			_phases.Dispose();
		}

		private void Apply(ADSRClipGeneratorMessage message)
		{
			switch (message.Command)
			{
				case ADSRClipGeneratorCommand.NoteOn:
					BeginNote();
					break;
				case ADSRClipGeneratorCommand.NoteOff:
					BeginRelease();
					break;
			}
		}

		private void BeginNote()
		{
			if (_retriggerMode == ADSRClipGeneratorRetriggerMode.Legato && IsActive())
			{
				BeginLegatoNote();
				return;
			}

			BeginRestartedNote();
		}

		private void BeginRestartedNote()
		{
			_segment = _clips.Start.HasSamples ? ADSRClipGeneratorSegment.Start : ADSRClipGeneratorSegment.Loop;
			_segmentPosition = 0d;
			_stage = ADSRClipGeneratorEnvelopeStage.Attack;
			_stageTime = 0f;
			_currentVolume = _phases.Attack.Volume.Evaluate(0f);
			_currentPitch = _phases.Attack.Pitch.Evaluate(0f);
		}

		private void BeginLegatoNote()
		{
			if (!GetCurrentClip().HasSamples)
			{
				_segment = _clips.Loop.HasSamples ? ADSRClipGeneratorSegment.Loop : ADSRClipGeneratorSegment.Silence;
				_segmentPosition = 0d;
			}

			_stage = ADSRClipGeneratorEnvelopeStage.Attack;
			_stageTime = 0f;
		}

		private void BeginRelease()
		{
			if (_stage == ADSRClipGeneratorEnvelopeStage.Release || _stage == ADSRClipGeneratorEnvelopeStage.Finished)
			{
				return;
			}

			_releaseVolumeBase = _currentVolume;
			_releasePitchBase = _currentPitch;
			_stage = ADSRClipGeneratorEnvelopeStage.Release;
			_stageTime = 0f;
			BeginReleasePlaybackSegment();
		}

		private void BeginReleasePlaybackSegment()
		{
			if (_clips.End.HasSamples)
			{
				_segment = ADSRClipGeneratorSegment.End;
				_segmentPosition = 0d;
				return;
			}

			if (_clips.Loop.HasSamples)
			{
				if (_segment != ADSRClipGeneratorSegment.Loop || !GetCurrentClip().HasSamples)
				{
					_segment = ADSRClipGeneratorSegment.Loop;
					_segmentPosition = WrapPosition(_segmentPosition, _clips.Loop.FrameCount);
				}

				return;
			}

			_segment = ADSRClipGeneratorSegment.Silence;
			_segmentPosition = 0d;
		}

		private void UpdateEnvelope(float sampleDelta)
		{
			switch (_stage)
			{
				case ADSRClipGeneratorEnvelopeStage.Attack:
					UpdatePhase(_phases.Attack, sampleDelta, ADSRClipGeneratorEnvelopeStage.Decay);
					break;
				case ADSRClipGeneratorEnvelopeStage.Decay:
					UpdatePhase(_phases.Decay, sampleDelta, ADSRClipGeneratorEnvelopeStage.Sustain);
					break;
				case ADSRClipGeneratorEnvelopeStage.Sustain:
					UpdateSustain(sampleDelta);
					break;
				case ADSRClipGeneratorEnvelopeStage.Release:
					UpdateRelease(sampleDelta);
					break;
			}
		}

		private void UpdatePhase(ADSRClipGeneratorPhaseData phase, float sampleDelta, ADSRClipGeneratorEnvelopeStage nextStage)
		{
			float normalizedTime = phase.Duration > 0f ? _stageTime / phase.Duration : 1f;
			ApplyEnvelopeTarget(phase.Volume.Evaluate(normalizedTime), phase.Pitch.Evaluate(normalizedTime));
			_stageTime += sampleDelta;

			if (_stageTime >= phase.Duration)
			{
				_stage = nextStage;
				_stageTime = 0f;
			}
		}

		private void UpdateSustain(float sampleDelta)
		{
			float normalizedTime = 1f;
			if (_phases.Sustain.Duration > 0f)
			{
				normalizedTime = _stageTime / _phases.Sustain.Duration;
				normalizedTime -= (float)Math.Floor(normalizedTime);
			}

			ApplyEnvelopeTarget(_phases.Sustain.Volume.Evaluate(normalizedTime), _phases.Sustain.Pitch.Evaluate(normalizedTime));
			_stageTime += sampleDelta;
		}

		private void UpdateRelease(float sampleDelta)
		{
			float normalizedTime = _phases.Release.Duration > 0f ? _stageTime / _phases.Release.Duration : 1f;
			ApplyEnvelopeTarget(_releaseVolumeBase * _phases.Release.Volume.Evaluate(normalizedTime),
				_releasePitchBase * _phases.Release.Pitch.Evaluate(normalizedTime));

			_stageTime += sampleDelta;

			if (_stageTime >= _phases.Release.Duration)
			{
				_stage = ADSRClipGeneratorEnvelopeStage.Finished;
				_currentVolume = 0f;
			}
		}

		private bool IsActive()
		{
			return _stage != ADSRClipGeneratorEnvelopeStage.Idle && _stage != ADSRClipGeneratorEnvelopeStage.Finished;
		}

		private void ApplyEnvelopeTarget(float targetVolume, float targetPitch)
		{
			if (_envelopeLerp >= 1f)
			{
				_currentVolume = targetVolume;
				_currentPitch = targetPitch;
				return;
			}

			_currentVolume += (targetVolume - _currentVolume) * _envelopeLerp;
			_currentPitch += (targetPitch - _currentPitch) * _envelopeLerp;
		}

		private static float Clamp01(float value)
		{
			if (value < 0f)
			{
				return 0f;
			}

			return value > 1f ? 1f : value;
		}

		private float ReadCurrentSample(int outputChannel)
		{
			return _segment switch
			{
				ADSRClipGeneratorSegment.Start => ReadSample(_clips.Start, _segmentPosition, outputChannel, false),
				ADSRClipGeneratorSegment.Loop => ReadSample(_clips.Loop, _segmentPosition, outputChannel, true),
				ADSRClipGeneratorSegment.End => ReadSample(_clips.End, _segmentPosition, outputChannel, false),
				_ => 0f
			};
		}

		private float ReadSample(ADSRClipSectionSampleBuffer clip, double position, int outputChannel, bool loop)
		{
			if (!clip.HasSamples)
			{
				return 0f;
			}

			int firstFrame = (int)Math.Floor(position);
			if (loop)
			{
				firstFrame = PositiveModulo(firstFrame, clip.FrameCount);
			}
			else if (firstFrame >= clip.FrameCount)
			{
				return 0f;
			}

			int secondFrame = firstFrame + 1;
			if (loop)
			{
				secondFrame = PositiveModulo(secondFrame, clip.FrameCount);
			}
			else if (secondFrame >= clip.FrameCount)
			{
				secondFrame = firstFrame;
			}

			int sourceChannel = outputChannel < clip.Channels ? outputChannel : clip.Channels - 1;
			float transition = (float)(position - Math.Floor(position));
			int firstSourceFrame = clip.BeginFrame + firstFrame;
			int secondSourceFrame = clip.BeginFrame + secondFrame;
			float firstSample = clip.Samples[firstSourceFrame * clip.Channels + sourceChannel];
			float secondSample = clip.Samples[secondSourceFrame * clip.Channels + sourceChannel];
			return firstSample + (secondSample - firstSample) * transition;
		}

		private void AdvanceSegment()
		{
			ADSRClipSectionSampleBuffer clip = GetCurrentClip();
			if (!clip.HasSamples)
			{
				return;
			}

			float pitch = _currentPitch > 0f ? _currentPitch : 0f;
			_segmentPosition += pitch * _pitchFactor * clip.Frequency / _sampleRate;

			if (_segment == ADSRClipGeneratorSegment.Loop)
			{
				_segmentPosition = WrapPosition(_segmentPosition, clip.FrameCount);
				return;
			}

			if (_segmentPosition < clip.FrameCount)
			{
				return;
			}

			if (_segment == ADSRClipGeneratorSegment.Start)
			{
				_segment = _clips.Loop.HasSamples ? ADSRClipGeneratorSegment.Loop : ADSRClipGeneratorSegment.Silence;
				_segmentPosition = 0d;
			}
			else if (_segment == ADSRClipGeneratorSegment.End)
			{
				if (_stage == ADSRClipGeneratorEnvelopeStage.Release || _stage == ADSRClipGeneratorEnvelopeStage.Finished)
				{
					_segment = ADSRClipGeneratorSegment.Finished;
				}
				else
				{
					_segment = _clips.Loop.HasSamples ? ADSRClipGeneratorSegment.Loop : ADSRClipGeneratorSegment.Silence;
					_segmentPosition = 0d;
				}
			}
		}

		private ADSRClipSectionSampleBuffer GetCurrentClip()
		{
			return _segment switch
			{
				ADSRClipGeneratorSegment.Start => _clips.Start,
				ADSRClipGeneratorSegment.Loop => _clips.Loop,
				ADSRClipGeneratorSegment.End => _clips.End,
				_ => default
			};
		}

		private void ClearFrom(ChannelBuffer buffer, int startFrame)
		{
			for (int frame = startFrame; frame < buffer.frameCount; frame++)
			{
				for (int channel = 0; channel < buffer.channelCount; channel++)
				{
					buffer[channel, frame] = 0f;
				}
			}
		}

		private double WrapPosition(double position, int frameCount)
		{
			if (frameCount <= 0)
			{
				return 0d;
			}

			double result = position % frameCount;
			return result < 0d ? result + frameCount : result;
		}

		private int PositiveModulo(int value, int divisor)
		{
			int result = value % divisor;
			return result < 0 ? result + divisor : result;
		}
	}
}