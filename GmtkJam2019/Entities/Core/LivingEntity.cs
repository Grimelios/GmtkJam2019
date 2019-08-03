using GmtkJam2019.Interfaces;

namespace GmtkJam2019.Entities.Core
{
	public abstract class LivingEntity : HybridEntity, ITargetable
	{
		public int Health { get; set; }
		public int MaxHealth { get; set; }

		protected virtual void OnDeath()
		{
		}

		public virtual void ApplyDamage(int damage)
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
