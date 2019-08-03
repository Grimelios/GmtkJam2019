using Engine.Core._3D;
using Engine.Interfaces;
using Engine.Interfaces._2D;
using Engine.Interfaces._3D;
using Engine.Shapes._2D;
using GlmSharp;
using GmtkJam2019.Physics;
using GmtkJam2019.Sensors;

namespace GmtkJam2019.Entities.Core
{
	public abstract class HybridEntity : IPositionable3D, IRotatable, IDynamic
	{
		private bool selfUpdate;

		protected vec3 position;

		public virtual vec3 Position
		{
			get => position;
			set
			{
				position = value;

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

		protected virtual Sprite3D CreateSprite3D(Scene scene, string filename)
		{
			var sprite = new Sprite3D(filename);
			sprite.Position = position;
			scene.Renderer.Add(sprite);

			return sprite;
		}

		protected HybridBody CreateBody(Scene scene, Shape2D shape, int height, bool isControlling = true)
		{
			var body = new HybridBody(shape, height, this);
			body.Position = position;
			scene.World.Add(body);

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

			return sensor;
		}

		public virtual void Initialize(Scene scene)
		{
			Scene = scene;
		}

		public virtual void Update(float dt)
		{
			if (ControllingBody != null)
			{
				selfUpdate = true;
				Position = ControllingBody.Position;
				selfUpdate = false;
			}
		}
	}
}
