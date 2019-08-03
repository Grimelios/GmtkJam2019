using System.Collections.Generic;
using Engine.Interfaces._3D;
using Engine.Shapes._2D;
using GlmSharp;

namespace GmtkJam2019.Sensors
{
	public class HybridSensor : IPositionable3D
	{
		public HybridSensor(Shape2D shape, int height, object owner)
		{
			Shape = shape;
			Height = height;
			Owner = owner;
			Contacts = new List<HybridSensor>();
		}

		public vec3 Position { get; set; }
		public Shape2D Shape { get; }
		public List<HybridSensor> Contacts { get; }

		public int Height { get; }
		public object Owner { get; }
	}
}
