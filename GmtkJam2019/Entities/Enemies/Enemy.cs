using Engine;
using Engine.Core._3D;
using Engine.Graphics;
using Engine.Shapes._2D;
using Engine.Utility;
using GlmSharp;
using GmtkJam2019.Entities.Core;

namespace GmtkJam2019.Entities.Enemies
{
	public abstract class Enemy : LivingEntity
	{
		private static EnemyData[] dataArray = JsonUtilities.Deserialize<EnemyData[]>("Enemies.json");

		public static Player Player { get; set; }

		private Texture texture;
		private EnemyData data;

		protected Enemy(EnemyTypes type)
		{
			data = dataArray[(int)type];
			texture = ContentCache.GetTexture(data.Texture, true);
		}

		public override Texture CollisionTexture => texture;
		public override Rectangle CollisionBounds { get; }

		public override void Initialize(Scene scene)
		{
			var sprite = CreateSprite3D(scene, texture);
			sprite.IsBillboarded = true;

			CreateBody(scene, new Circle(data.Radius), data.Health);

			base.Initialize(scene);
		}

		public override void ApplyDamage(int damage, vec3 direction)
		{
			base.ApplyDamage(damage, direction);
		}
	}
}
