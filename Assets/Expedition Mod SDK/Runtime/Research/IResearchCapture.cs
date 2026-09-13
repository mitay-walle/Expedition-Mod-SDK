namespace Research
{
	public interface IResearchCapture
	{
		void TakePhoto(CameraToolConfig tool);
		void ToggleRecording(RecorderToolConfig tool);
		void PlaceAnalyzer(AnalyzerToolConfig tool);
	}
}
