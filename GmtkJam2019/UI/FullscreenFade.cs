using Engine;
using Engine.Core;
using Engine.Graphics._2D;
using Engine.Shapes._2D;
using Engine.Timing;
using Engine.UI;

namespace GmtkJam2019.UI
{
	public class FullscreenFade : CanvasElement
	{
		private const float FadeTime = 1.2f;

		private SingleTimer timer;
		private Color color;

		public FullscreenFade()
		{
			timer = new SingleTimer(time => { MarkedForDestruction = true; }, FadeTime);
			timer.Tick = progress => { color = Color.Lerp(Color.Black, Color.Transparent, progress); };
			timer.Paused = true;

			Components.Add(timer);
			color = Color.Black;
		}

		public void BeginFade()
		{
			timer.Paused = false;
		}

		public override void Draw(SpriteBatch sb)
		{
			int w = Resolution.WindowWidth;
			int h = Resolution.WindowHeight;

			sb.Fill(new Rectangle(w / 2f, h / 2f, w, h), color);
		}
	}
}
