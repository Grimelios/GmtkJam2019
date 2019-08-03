using System.Collections.Generic;

namespace GmtkJam2019.Physics
{
	public class HybridWorld
	{
		private const int MaxSteps = 8;
		private const float PhysicsStep = 1f / 120;
		private const float Gravity = 10;

		private List<HybridBody> bodies;

		private float accumulator;

		public HybridWorld()
		{
			bodies = new List<HybridBody>();
		}

		public void Add(HybridBody body)
		{
			bodies.Add(body);
		}

		public void Step(float dt)
		{
			accumulator += dt;

			int steps = 0;

			while (accumulator >= PhysicsStep && steps < MaxSteps)
			{
				Solve(PhysicsStep);

				accumulator -= PhysicsStep;
				steps++;
			}
		}

		private void Solve(float step)
		{
			foreach (var body in bodies)
			{
				var v = body.Velocity;

				if (body.AffectedByGravity)
				{
					//v.y -= Gravity * step;
					body.Velocity = v;
				}

				body.Position += v * step;
			}
		}
	}
}
