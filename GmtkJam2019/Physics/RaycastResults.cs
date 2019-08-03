using GlmSharp;
using GmtkJam2019.Entities.Core;

namespace GmtkJam2019.Physics
{
	public class RaycastResults
	{
		public RaycastResults(vec3 position, vec3 normal, HybridEntity entity)
		{
			Position = position;
			Normal = normal;
			Entity = entity;
		}

		public vec3 Position { get; }
		public vec3 Normal { get; }

		public HybridEntity Entity { get; }
	}
}
