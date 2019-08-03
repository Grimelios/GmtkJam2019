using Engine.Core._3D;
using GmtkJam2019.Entities.Core;

namespace GmtkJam2019.Entities.Enemies
{
	public abstract class Enemy : LivingEntity
	{
		public static Player Player { get; set; }

		protected override Sprite3D CreateSprite3D(Scene scene, string filename)
		{
			var sprite = base.CreateSprite3D(scene, filename);
			sprite.IsBillboarded = true;

			return sprite;
		}
	}
}
