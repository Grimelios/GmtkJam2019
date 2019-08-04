using Engine.Shapes._3D;
using GlmSharp;
using GmtkJam2019.Entities;
using GmtkJam2019.Entities.Core;
using GmtkJam2019.Physics;

namespace GmtkJam2019.Weapons
{
	public class Pistol : Weapon
	{
		private const float Range = 100000;
		private const int Damage = 10;

		public Pistol() : base(0)
		{
		}

		public override void PrimaryFire(Scene scene, vec3 direction)
		{
			var player = (Player)Owner;
			var results = PhysicsHelper.Raycast(player.Eye, direction, Range, scene);

			if (results != null)
			{
				var p = results.Position;
				player.ShotLine = new Line3D(Position, p);
				player.NormalLine = new Line3D(p, p + results.Normal * 1.5f);

				results.Target?.ApplyDamage(Damage, direction, results.IsHeadshot);
			}

			base.PrimaryFire(scene, direction);
		}
	}
}
