using Engine.Core;
using Engine.Core._2D;
using Engine.Graphics._2D;
using Engine.UI;
using GlmSharp;

namespace GmtkJam2019.UI
{
	public class Reticle : CanvasElement
	{
		private const int Length = 12;

		public Reticle()
		{
			Anchor = Alignments.Center;
		}

		public override void Draw(SpriteBatch sb)
		{
			vec2 left = Location - new ivec2(Length, 0);
			vec2 right = Location + new ivec2(Length, 0);
			vec2 top = Location - new ivec2(0, Length);
			vec2 bottom = Location + new ivec2(0, Length);

			Color color = Color.White;
			color.A = 80;

			sb.DrawLine(left, right, color);
			sb.DrawLine(top, bottom, color);
		}
	}
}
