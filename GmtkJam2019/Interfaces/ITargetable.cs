using Engine.Graphics;
using Engine.Interfaces._2D;
using Engine.Shapes._2D;
using GlmSharp;

namespace GmtkJam2019.Interfaces
{
	public interface ITargetable : IRotatable
	{
		Texture CollisionTexture { get; }
		Rectangle CollisionBounds { get; }

		void ApplyDamage(int damage, vec3 direction);
	}
}
