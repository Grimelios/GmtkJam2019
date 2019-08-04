using Engine.Graphics;
using GlmSharp;
using GmtkJam2019.Interfaces;

namespace GmtkJam2019.Entities.Core
{
	public abstract class LivingEntity : HybridEntity, ITargetable
	{
		public int Health { get; set; }
		public int MaxHealth { get; set; }

		public virtual Texture CollisionTexture => null;

		public virtual vec2 CollisionBounds => vec2.Zero;
		public virtual vec2 Scale { get; set; }

		protected virtual void OnDeath()
		{
		}

		public virtual void ApplyDamage(int damage, vec3 direction, bool isHeadshot)
		{
			Health -= damage;

			if (Health <= 0)
			{
				Health = 0;
				OnDeath();
			}
		}
	}
}
