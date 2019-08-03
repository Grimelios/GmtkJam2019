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

		public vec3 Position
		{
			get => position;
			set
			{
				position = value;

				if (!selfUpdate)
				{
					Body.Position = value;
				}
			}
		}

		public HybridBody Body { get; protected set; }
		public Scene Scene { get; set; }

		public float Rotation { get; set; }

		public bool IsMarkedForDestruction { get; set; }

		protected virtual Sprite3D CreateSprite3D(Scene scene, string filename)
		{
			var sprite = new Sprite3D(filename);
			scene.Renderer.Add(sprite);

			return sprite;
		}

		protected HybridSensor CreateSensor(Scene scene, Shape2D shape, int height)
		{
			var sensor = new HybridSensor(shape, height, this);
			scene.Space.Add(sensor);

			return sensor;
		}

		public virtual void Initialize(Scene scene)
		{
			Scene = scene;
		}

		public virtual void Update(float dt)
		{
			if (Body != null)
			{
				selfUpdate = true;
				Position = Body.Position;
				selfUpdate = false;
			}
		}
	}
}
