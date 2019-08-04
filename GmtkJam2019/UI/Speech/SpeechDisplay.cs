using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.Core;
using Engine.Core._2D;
using Engine.Graphics._2D;
using Engine.Timing;
using Engine.UI;
using Engine.Utility;
using GlmSharp;
using GmtkJam2019.Sound;

namespace GmtkJam2019.UI.Speech
{
	public class SpeechDisplay : CanvasElement
	{
		private const int GlyphsPerSecond = 24;
		private const float FadeTime = 0.8f;

		private List<SpeechGlyph> glyphs;
		private RepeatingTimer glyphTimer;
		private SpriteFont font;
		private StringBuilder revealedLine;
		private vec2 lineStart;

		private string[] lines;
		private string currentLine;
		private int lineIndex;
		private int glyphIndex;

		public SpeechDisplay()
		{
			font = ContentCache.GetFont("Default");
			glyphs = new List<SpeechGlyph>();
			glyphTimer = new RepeatingTimer(RevealNext, 1f / GlyphsPerSecond);
			glyphTimer.Repeatable = true;
			revealedLine = new StringBuilder();

			Components.Add(glyphTimer);

			Anchor = Alignments.Top;
			Bounds = new Bounds2D(1000, 100);
			Centered = true;
		}

		private bool RevealNext(float progress)
		{
			char c = currentLine[glyphIndex];

			glyphIndex++;
			revealedLine.Append(c);

			if (c == ' ')
			{
				return true;
			}

			var glyph = new SpeechGlyph(font, c);
			glyph.Position = lineStart + new vec2(font.Measure(revealedLine.ToString()).x, 0);
			glyphs.Add(glyph);

			if (glyphIndex == currentLine.Length - 1)
			{
				glyphIndex = 0;
				lineIndex++;

				if (lineIndex == lines.Length)
				{
					lineIndex = 0;

					return false;
				}

				currentLine = lines[lineIndex];
				lineStart.y += font.Size;
				revealedLine.Clear();
			}

			Canvas.AudioPlayback.Play(SoundEffects.Speech);

			return true;
		}

		public void Refresh(string text)
		{
			lines = Utilities.WrapLines(text, font, Bounds.Width);

			int width = -1;
			int height = -1;

			ivec2 v = ivec2.Zero;

			if (lines.Length == 1)
			{
				ivec2 dimensions = font.MeasureLiteral(lines[0], out ivec2 offset);

				width = dimensions.x - offset.x;
				height = dimensions.y - offset.y;
			}
			else
			{
				for (int i = 0; i < lines.Length; i++)
				{
					ivec2 dimensions = font.MeasureLiteral(lines[i], out ivec2 offset);

					if (dimensions.x > width)
					{
						width = dimensions.x;
						v.x = offset.x;
					}

					int size = font.Size;
					int y = offset.y;

					if (i == 0)
					{
						height += size - y;
						v.y = y;
					}
					else if (i == lines.Length - 1)
					{
						height += dimensions.y + y;
					}
					else
					{
						height += size;
					}
				}
			}

			lineIndex = 0;
			lineStart = Bounds.Center - new vec2(width, height) / 2 - v;
			currentLine = lines[0];

			glyphIndex = 0;
			glyphTimer.Paused = false;
			glyphTimer.Elapsed = glyphTimer.Duration;
		}

		public void BeginFade()
		{
			var timer = new SingleTimer(time => { MarkedForDestruction = true; }, FadeTime);
			timer.Tick = progress =>
			{
				Color color = Color.Lerp(Color.White, Color.Transparent, progress);
				glyphs.ForEach(g => g.Color = color);
			};

			Components.Add(timer);
		}

		public override void Draw(SpriteBatch sb)
		{
			glyphs.ForEach(g => g.Draw(sb));
		}
	}
}
