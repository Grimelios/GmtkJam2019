using Engine.Core;
using Engine.Core._2D;
using Engine.Graphics._2D;
using Engine.UI;
using GlmSharp;

namespace GmtkJam2019.UI
{
	public class PlayerVisionDisplay : CanvasElement
	{
		private Sprite[] sprites;
		private Color[] colors;

		private bool isEyeRemoved;

		public PlayerVisionDisplay()
		{
			sprites = new Sprite[2];
			Anchor = Alignments.Top;
			Offset = new ivec2(0, 100);
			Centered = true;

			for (int i = 0; i < 2; i++)
			{
				sprites[i] = new Sprite("UI.png", new Bounds2D(0, 0, 119, 73));
			}

			sprites[1].Mods = SpriteModifiers.FlipHorizontal;

			colors = new []
			{
				Color.White,
				Color.Red,
				Color.Yellow,
				Color.Green
			};
		}

		public int Health
		{
			set
			{
				int limit = isEyeRemoved ? 1 : 2;

				for (int i = 0; i < limit; i++)
				{
					sprites[i].Color = colors[value];
				}
			}
		}

		public override ivec2 Location
		{
			get => base.Location;
			set
			{
				for (int i = 0; i < sprites.Length; i++)
				{
					sprites[i].Position = value + new vec2(90 * (i == 0 ? -1 : 1), 0);
				}

				base.Location = value;
			}
		}

		public void RemoveEye()
		{
			var sprite = sprites[1];
			sprite.SourceRect = new Bounds2D(118, 12, 115, 61);
			sprite.Mods = SpriteModifiers.None;
			sprite.Position += new vec2(0, 6);

			isEyeRemoved = true;
		}

		public override void Draw(SpriteBatch sb)
		{
			foreach (Sprite sprite in sprites)
			{
				sprite.Draw(sb);
			}
		}
	}
}
