using Engine.Interfaces;
using Engine.Interfaces._3D;
using Engine.Timing;
using GlmSharp;
using GmtkJam2019.Entities.Core;

namespace GmtkJam2019.Weapons
{
	public abstract class Weapon : ITransformable3D, IDynamic
	{
		private float cooldown;

		private SingleTimer cooldownTimer;

		protected Weapon(float cooldown)
		{
			this.cooldown = cooldown;

			if (cooldown != 0)
			{
				cooldownTimer = new SingleTimer(time => { OnCooldown = false; }, cooldown);
				cooldownTimer.Paused = true;
				cooldownTimer.Repeatable = true;
			}
		}

		public vec3 Position { get; set; }
		public quat Orientation { get; set; }

		// Allowing the owner to be set externally simplifies construction and potentially allows weapons to be picked
		// up by various other creatures during the game.
		public LivingEntity Owner { get; set; }

		public bool OnCooldown { get; private set; }

		public virtual void PrimaryFire(Scene scene, vec3 direction)
		{
			// Some weapons (like the pistol) can be fired as quickly as the player can click.
			if (cooldown == 0)
			{
				return;
			}

			cooldownTimer.Paused = false;
			OnCooldown = true;
		}

		public void Update(float dt)
		{
			cooldownTimer?.Update(dt);
		}

		public void SetTransform(vec3 position, quat orientation)
		{
			Position = position;
			Orientation = orientation;
		}
	}
}
