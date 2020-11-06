namespace VisionGame
{
	public class GameCamera : CameraScript
	{
		public override void Awake ()
		{
		}

		public override void DoUpdate ()
		{
			camera.Render();
		}
	}
}