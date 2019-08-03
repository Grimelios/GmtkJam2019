using Engine.Graphics;
using Engine.Shapes._2D;
using GlmSharp;
using GmtkJam2019.Interfaces;

namespace GmtkJam2019.Entities.Core
{
	public abstract class LivingEntity : HybridEntity, ITargetable
	{
		public int Health { get; set; }
		public int MaxHealth { get; set; }

		public virtual Texture CollisionTexture => null;
		public virtual Rectangle CollisionBounds => null;

		protected virtual void OnDeath()
		{
		}

		public virtual void ApplyDamage(int damage, vec3 direction)
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
