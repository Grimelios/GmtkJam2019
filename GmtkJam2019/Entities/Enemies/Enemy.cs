using Engine;
using Engine.Core;
using Engine.Core._3D;
using Engine.Graphics;
using Engine.Shapes._2D;
using Engine.Timing;
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
		private Sprite3D sprite;
		private EnemyData data;
		private SingleTimer tintTimer;
		private vec2 collisionBounds;
		private vec2 scale;

		// This means that the enemy was just hit and the sprite color is still transitioning back to white. If hit
		// again in this state, the existing timer is simply reset (rather than creating a new one).
		private bool damageActive;

		protected Enemy(EnemyTypes type)
		{
			data = dataArray[(int)type];
			texture = ContentCache.GetTexture(data.Texture, true);
			collisionBounds = new vec2(texture.Width, texture.Height) / Sprite3D.PixelDivisor;
		}

		public override Texture CollisionTexture => texture;

		public override vec2 CollisionBounds => collisionBounds;
		public override vec2 Scale
		{
			get => scale;
			set
			{
				scale = value;
				collisionBounds = new vec2(texture.Width, texture.Height) / Sprite3D.PixelDivisor * value;

				if (sprite != null)
				{
					sprite.Scale = value;
				}
			}
		}

		public override void Initialize(Scene scene)
		{
			sprite = CreateSprite3D(scene, texture);
			sprite.IsBillboarded = true;

			CreateBody(scene, new Circle(data.Radius), data.Health);

			base.Initialize(scene);
		}

		public override void ApplyDamage(int damage, vec3 direction)
		{
			if (damageActive)
			{
				tintTimer.Elapsed = 0;
			}
			else
			{
				tintTimer = new SingleTimer(time => { damageActive = false; }, 0.35f);
				tintTimer.Tick = progress => { sprite.Color = Color.Lerp(Color.Red, Color.White,
					progress * progress); };

				components.Add(tintTimer);
				damageActive = true;
			}

			base.ApplyDamage(damage, direction);
		}

		public override void Update(float dt)
		{
			// This specific kind of billboarding works for a Doom-style FPS (where sprites only billboard along a
			// vertical axis).
			Rotation = Utilities.Angle(Position.swizzle.xz, Scene.Camera.Position.swizzle.xz);
			sprite.Orientation = quat.FromAxisAngle(-Rotation + Constants.PiOverTwo, vec3.UnitY);
			sprite.Scale = scale;

			base.Update(dt);
		}
	}
}
