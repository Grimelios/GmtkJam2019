using Engine.Interfaces._2D;
using Engine.Interfaces._3D;
using Engine.Shapes._2D;
using GlmSharp;

namespace GmtkJam2019.Physics
{
	public class HybridBody : IPositionable3D, IRotatable
	{
		private float elevation;

		public HybridBody(Shape2D shape, int height, bool isStatic, object owner = null)
		{
			Shape = shape;
			Height = height;
			Owner = owner;
			AffectedByGravity = true;
			IsEnabled = true;
			IsStatic = isStatic;
		}

		public Shape2D Shape { get; }

		public vec3 Position
		{
			get
			{
				vec2 p = Shape.Position;

				return new vec3(p.x, elevation, p.y);
			}

			set
			{
				Shape.Position = value.swizzle.xz;
				elevation = value.y;
			}
		}

		public vec3 Velocity { get; set; }

		public object Owner { get; set; }
		public int Height { get; set; }
		
		public float Rotation { get; set; }

		public bool AffectedByGravity { get; set; }
		public bool IsStatic { get; set; }
		public bool IsEnabled { get; set; }
	}
}
