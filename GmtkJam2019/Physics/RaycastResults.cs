using GlmSharp;
using GmtkJam2019.Interfaces;

namespace GmtkJam2019.Physics
{
	public class RaycastResults
	{
		public RaycastResults(vec3 position, vec3 normal, ITargetable target)
		{
			Position = position;
			Normal = normal;
			Target = target;
		}

		public vec3 Position { get; }
		public vec3 Normal { get; }

		public ITargetable Target { get; }
	}
}
