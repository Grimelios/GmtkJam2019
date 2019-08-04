using Engine.Graphics;
using Engine.Interfaces._2D;
using Engine.Interfaces._3D;
using GlmSharp;

namespace GmtkJam2019.Interfaces
{
	public interface ITargetable : IPositionable3D, IRotatable, IScalable2D
	{
		Texture CollisionTexture { get; }
		vec2 CollisionBounds { get; }

		void ApplyDamage(int damage, vec3 direction, bool isHeadshot);
	}
}
