using Engine.Core._2D;
using Engine.Graphics._2D;
using Engine.UI;
using GlmSharp;

namespace GmtkJam2019.UI
{
	public class PlayerVisionDisplay : CanvasElement
	{
		private Sprite[] sprites;

		public PlayerVisionDisplay()
		{
			Eyes = 2;
			sprites = new Sprite[2];
			Anchor = Alignments.Top;
			Offset = new ivec2(0, 150);
			Centered = true;

			for (int i = 0; i < 2; i++)
			{
				sprites[i] = new Sprite("UI.png", new Bounds2D(0, 0, 119, 73));
			}

			sprites[1].Mods = SpriteModifiers.FlipHorizontal;
		}

		public int Eyes { get; set; }
		public int Health { get; set; }

		public override ivec2 Location
		{
			get => base.Location;
			set
			{
				for (int i = 0; i < sprites.Length; i++)
				{
					sprites[i].Position = value + new vec2(95 * (i == 0 ? -1 : 1), 0);
				}

				base.Location = value;
			}
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
