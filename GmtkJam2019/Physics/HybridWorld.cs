using System.Collections.Generic;
using Engine.Interfaces;

namespace GmtkJam2019.Physics
{
	public class HybridWorld : IDynamic
	{
		private const int MaxSteps = 8;

		private List<HybridBody> bodies;

		public HybridWorld()
		{
			bodies = new List<HybridBody>();
		}

		public void Add(HybridBody body)
		{
		}

		public void Update(float dt)
		{
			foreach (var body in bodies)
			{
				body.Position += body.Velocity * dt;
			}
		}
	}
}
