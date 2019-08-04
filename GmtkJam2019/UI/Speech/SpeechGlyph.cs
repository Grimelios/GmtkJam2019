using Engine.Core;
using Engine.Core._2D;
using Engine.Graphics._2D;
using Engine.Interfaces;
using Engine.Interfaces._2D;
using GlmSharp;

namespace GmtkJam2019.UI.Speech
{
	public class SpeechGlyph : IPositionable2D, IColorable, IRenderable2D
	{
		private SpriteText text;

		public SpeechGlyph(SpriteFont font, char value)
		{
			text = new SpriteText(font, value.ToString());
		}

		public vec2 Position
		{
			get => text.Position;
			set => text.Position = value;
		}

		public Color Color
		{
			get => text.Color;
			set => text.Color = value;
		}

		public void Draw(SpriteBatch sb)
		{
			text.Draw(sb);
		}
	}
}
