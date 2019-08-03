using System;
using System.Collections.Generic;
using Engine;
using Engine.Core;
using Engine.Core._2D;
using Engine.Core._3D;
using Engine.Graphics;
using Engine.Interfaces;
using Engine.Interfaces._2D;
using Engine.Interfaces._3D;
using Engine.Shapes._2D;
using GlmSharp;
using GmtkJam2019.Physics;
using GmtkJam2019.Sensors;

namespace GmtkJam2019.Entities.Core
{
	public abstract class HybridEntity : IPositionable3D, IRotatable, IDynamic, IDisposable
	{
		private List<IPositionable3D> attachments;

		private bool selfUpdate;

		protected vec3 position;
		protected ComponentCollection components;

		protected HybridEntity()
		{
			attachments = new List<IPositionable3D>();
			components = new ComponentCollection();
		}

		public virtual vec3 Position
		{
			get => position;
			set
			{
				position = value;
				attachments.ForEach(a => a.Position = value);

				if (!selfUpdate && ControllingBody != null)
				{
					ControllingBody.Position = value;
				}
			}
		}

		public HybridBody ControllingBody { get; protected set; }
		public Scene Scene { get; set; }

		public float Rotation { get; set; }

		public bool IsMarkedForDestruction { get; set; }

		protected void Attach(IPositionable3D item)
		{
			attachments.Add(item);
		}

		protected Sprite3D CreateSprite3D(Scene scene, string filename)
		{
			return CreateSprite3D(scene, ContentCache.GetTexture(filename));
		}

		protected Sprite3D CreateSprite3D(Scene scene, Texture texture)
		{
			var sprite = new Sprite3D(texture);
			sprite.Position = position;
			sprite.Camera = scene.Camera;

			scene.Renderer.Add(sprite);
			Attach(sprite);

			return sprite;
		}

		protected HybridBody CreateBody(Scene scene, Shape2D shape, int height, bool isControlling = true)
		{
			var body = new HybridBody(shape, height, false, this);
			body.Position = position;

			scene.World.Add(body);

			// Controlling bodies are positioned manually.
			if (!isControlling)
			{
				Attach(body);
			}

			if (isControlling)
			{
				ControllingBody = body;
			}

			return body;
		}

		protected HybridSensor CreateSensor(Scene scene, Shape2D shape, int height)
		{
			var sensor = new HybridSensor(shape, height, this);
			sensor.Position = position;

			scene.Space.Add(sensor);
			Attach(sensor);

			return sensor;
		}

		public virtual void Initialize(Scene scene)
		{
			Scene = scene;
		}

		public void Dispose()
		{
			if (ControllingBody != null)
			{
				Scene.World.Remove(ControllingBody);
			}
		}

		public virtual void Update(float dt)
		{
			components.Update(dt);

			if (ControllingBody != null)
			{
				selfUpdate = true;
				Position = ControllingBody.Position + new vec3(0, ControllingBody.Height / 2f, 0);
				selfUpdate = false;
			}
		}
	}
}
