﻿using System.Collections.Generic;
using Engine;
using Engine.Core;
using Engine.Core._2D;
using Engine.Core._3D;
using Engine.Graphics._2D;
using Engine.Graphics._3D;
using Engine.Interfaces;
using Engine.Messaging;
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
using static Engine.GL;
using static Engine.GLFW;

namespace GmtkJam2019
{
	public class MainGame : Game, IReceiver
	{
		private SpriteBatch sb;
		private RenderTarget mainTarget;
		private Sprite mainSprite;
		private Sprite3D sprite3D;
		private Camera3D camera;
		private Canvas canvas;
		private HybridSpace space;
		private HybridWorld world;
		private Scene scene;
		private GameSettings settings;

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
			//camera.IsOrthographic = true;

			sb = new SpriteBatch();
			space = new HybridSpace();
			world = new HybridWorld();

			mainTarget = new RenderTarget(Resolution.RenderWidth, Resolution.RenderHeight,
				RenderTargetFlags.Color | RenderTargetFlags.Depth);
			mainSprite = new Sprite(mainTarget, null, Alignments.Left | Alignments.Top);
			mainSprite.Mods = SpriteModifiers.FlipVertical;

			settings = new GameSettings();

			PlayerVisionDisplay visionDisplay = new PlayerVisionDisplay();
			Player player = new Player(camera);
			player.Position = new vec3(0, 4, 4);
			player.VisionDisplay = visionDisplay;
			player.Controller.Settings = settings;

			sprite3D = new Sprite3D("Link.png");

			scene = new Scene
			{
				Camera = camera,
				Space = space,
				World = world
			};

			scene.Add(player);

			var renderer = scene.Renderer;
			renderer.Add(new Model("Demo.obj"));
			renderer.Add(sprite3D);
			renderer.Light.Direction = Utilities.Normalize(new vec3(1, -0.25f, 0));

			// Quick hack to give all enemies easy access to the player.
			Enemy.Player = player;

			canvas = new Canvas();
			canvas.Add(visionDisplay);

			visionDisplay.RemoveEye();

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

		protected override void Update(float dt)
		{
			scene.Update(dt);
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
