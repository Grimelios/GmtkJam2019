using Engine;
using Engine.Core._2D;
using Engine.Graphics._2D;
using Engine.UI;
using Engine.View;
using GmtkJam2019.Entities;
using GmtkJam2019.UI;
using static Engine.GL;

namespace GmtkJam2019
{
	public class MainGame : Game
	{
		private SpriteBatch sb;
		private RenderTarget mainTarget;
		private Sprite mainSprite;
		private Camera3D camera;
		private Canvas canvas;

		public MainGame() : base("GMTK Jam 2019 - Grimelios")
		{
			glClearColor(0, 0, 0, 1);
			glEnable(GL_BLEND);
			glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
			glPrimitiveRestartIndex(Constants.RestartIndex);

			Properties.Load("Render.properties");

			camera = new Camera3D();
			camera.IsOrthographic = true;

			sb = new SpriteBatch();

			mainTarget = new RenderTarget(Resolution.RenderWidth, Resolution.RenderHeight,
				RenderTargetFlags.Color | RenderTargetFlags.Depth);
			mainSprite = new Sprite(mainTarget, null, Alignments.Left | Alignments.Top);
			mainSprite.Mods = SpriteModifiers.FlipVertical;

			PlayerVisionDisplay visionDisplay = new PlayerVisionDisplay();
			Player player = new Player();
			player.VisionDisplay = visionDisplay;

			canvas = new Canvas();
			canvas.Add(visionDisplay);
		}

		protected override void Update(float dt)
		{
			camera.Update(dt);
		}

		protected override void Draw()
		{
			// Render 3D targets.
			glEnable(GL_DEPTH_TEST);
			glEnable(GL_CULL_FACE);
			glDepthFunc(GL_LEQUAL);

			mainTarget.Apply();

			// Render 2D targets.
			glDisable(GL_DEPTH_TEST);
			glDisable(GL_CULL_FACE);
			glDepthFunc(GL_NEVER);

			canvas.DrawTargets(sb);

			// Draw to the main screen.
			glBindFramebuffer(GL_FRAMEBUFFER, 0);
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
			glViewport(0, 0, (uint)Resolution.WindowWidth, (uint)Resolution.WindowHeight);

			sb.ApplyTarget(null);
			mainSprite.Draw(sb);
			canvas.Draw(sb);
			sb.Flush();
		}
	}
}
