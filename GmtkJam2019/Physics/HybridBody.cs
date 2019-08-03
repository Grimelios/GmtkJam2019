using Engine.Shapes._2D;
using GlmSharp;

namespace GmtkJam2019.Physics
{
	public class HybridBody
	{
		public HybridBody(Shape2D shape, int height, object owner)
		{
			Shape = shape;
			Height = height;
			Owner = owner;
		}

		public vec3 Position { get; set; }
		public vec3 Velocity { get; set; }
		public Shape2D Shape { get; }

		public object Owner { get; set; }

		public int Height { get; set; }
	}
}
