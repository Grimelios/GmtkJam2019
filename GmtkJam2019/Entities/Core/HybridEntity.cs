using Engine.Interfaces;
using Engine.Interfaces._2D;
using Engine.Interfaces._3D;
using GlmSharp;
using GmtkJam2019.Physics;

namespace GmtkJam2019.Entities.Core
{
	public abstract class HybridEntity : IPositionable3D, IRotatable, IDynamic
	{
		private vec3 position;

		private bool selfUpdate;

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

		public float Rotation { get; set; }

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
