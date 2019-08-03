using System;
using System.Collections.Generic;
using Engine;
using Engine.Core;
using Engine.Core._2D;
using Engine.Core._3D;
using Engine.Graphics._2D;
using Engine.Graphics._3D;
using Engine.Interfaces;
using Engine.Messaging;
using Engine.Shapes._2D;
using Engine.UI;
using Engine.Utility;
using Engine.View;
using GlmSharp;
using GmtkJam2019.Entities;
using GmtkJam2019.Entities.Core;
using GmtkJam2019.Entities.Enemies;
using GmtkJam2019.Physics;
using GmtkJam2019.Sensors;
using GmtkJam2019.Settings;
using GmtkJam2019.UI;
using GmtkJam2019.Weapons;
using static Engine.GL;
using static Engine.GLFW;

namespace GmtkJam2019
{
	public class MainGame : Game, IReceiver
	{
		private SpriteBatch sb;
		private RenderTarget mainTarget;
		private Sprite mainSprite;
		private Camera3D camera;
		private Canvas canvas;
		private HybridSpace space;
		private HybridWorld world;
		private Scene scene;
		private GameSettings settings;
		private PrimitiveRenderer3D primitives;
		private Player player;

		public MainGame() : base("GMTK Jam 2019 - Grimelios")
		{
			glClearColor(0, 0, 0, 1);
			glEnable(GL_BLEND);
			glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
			glPrimitiveRestartIndex(Constants.RestartIndex);

			// This hides the cursor and lets it function properly for an FPS.
			glfwSetInputMode(window.Address, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

			Properties.Load("Render.properties");

			camera = new Camera3D();
			camera.OrthoWidth = Properties.GetFloat("camera.ortho.width");
			camera.OrthoHeight = Properties.GetFloat("camera.ortho.height");
			camera.NearPlane = Properties.GetFloat("camera.near.plane");
			camera.FarPlane = Properties.GetFloat("camera.far.plane");

			primitives = new PrimitiveRenderer3D(camera, 10000, 1000);

			vec2[] border =
			{
				new vec2(-1, -1), 
				new vec2(1, -1), 
				new vec2(1, 1), 
				new vec2(-1, 1)
			};

			for (int i = 0; i < border.Length; i++)
			{
				border[i] *= 10;
			}

			sb = new SpriteBatch();
			space = new HybridSpace();
			world = new HybridWorld();
			world.Add(new HybridBody(new Rectangle(5, 5, 3, 3), 10, true));
			world.Add(new HybridBody(new Rectangle(-5, 5, 3, 3), 10, true));
			world.Add(new HybridBody(new Rectangle(5, -5, 3, 3), 10, true));
			world.Add(new HybridBody(new Rectangle(-5, -5, 3, 3), 10, true));

			for (int i = 0; i < border.Length; i++)
			{
				vec2 p1 = border[i];
				vec2 p2 = border[(i + 1) % 4];

				world.Add(new HybridBody(new Line2D(p1, p2), 10, true));
			}

			mainTarget = new RenderTarget(Resolution.RenderWidth, Resolution.RenderHeight,
				RenderTargetFlags.Color | RenderTargetFlags.Depth);
			mainSprite = new Sprite(mainTarget, null, Alignments.Left | Alignments.Top);
			mainSprite.Mods = SpriteModifiers.FlipVertical;

			settings = new GameSettings();

			PlayerVisionDisplay visionDisplay = new PlayerVisionDisplay();

			player = new Player(camera);
			player.Position = new vec3(0, 0, 4);
			player.VisionDisplay = visionDisplay;
			player.Controller.Settings = settings;
			player.Equip(new Pistol());

			Model map = new Model("Demo.obj");

			scene = new Scene
			{
				Camera = camera,
				Space = space,
				World = world
			};

			scene.Add(player);
			scene.Add(new Monoclops());
			scene.WorldMesh = map.Mesh;

			var renderer = scene.Renderer;
			renderer.Add(map);
			renderer.Light.Direction = Utilities.Normalize(new vec3(1, -0.25f, 0));

			// Quick hack to give all enemies easy access to the player.
			Enemy.Player = player;

			canvas = new Canvas();
			canvas.Add(visionDisplay);
			canvas.Add(new Reticle());

			MessageSystem.Subscribe(this, CoreMessageTypes.ResizeWindow, (messageType, data, dt) =>
			{
				mainSprite.ScaleTo(Resolution.WindowWidth, Resolution.WindowHeight);
			});

			// Calling this function here is required to ensure that all classes receive initial resize messages.
			MessageSystem.ProcessChanges();
			MessageSystem.Send(CoreMessageTypes.ResizeRender, Resolution.RenderDimensions);
			MessageSystem.Send(CoreMessageTypes.ResizeWindow, Resolution.WindowDimensions);
		}

		public List<MessageHandle> MessageHandles { get; set; }

		public void Dispose()
		{
			MessageSystem.Unsubscribe(this);
		}

		private float rotation;

		protected override void Update(float dt)
		{
			float x = (float)Math.Cos(rotation);
			float y = (float)Math.Sin(rotation);

			rotation += dt / 3;

			scene.Update(dt);
			scene.Renderer.Light.Direction = Utilities.Normalize(new vec3(x, -0.25f, y));
			world.Step(dt);
			camera.Update(dt);

			MessageSystem.ProcessChanges();
		}

		protected override void Draw()
		{
			// Render 3D targets.
			glEnable(GL_DEPTH_TEST);
			glEnable(GL_CULL_FACE);
			glDepthFunc(GL_LEQUAL);

			scene.Renderer.DrawTargets();
			mainTarget.Apply();
			scene.Draw();

			if (player.AimLine != null)
			{
				primitives.Draw(player.AimLine, Color.Yellow);
			}

			if (player.ShotLine != null)
			{
				primitives.Draw(player.ShotLine, Color.Red);
				primitives.Draw(player.NormalLine, Color.Green);
			}

			primitives.Flush();

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
