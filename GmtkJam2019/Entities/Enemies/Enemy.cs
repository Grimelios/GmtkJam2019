using Engine;
using Engine.Core;
using Engine.Core._2D;
using Engine.Core._3D;
using Engine.Graphics;
using Engine.Shapes._2D;
using Engine.Smoothers._3D;
using Engine.Timing;
using Engine.Utility;
using GlmSharp;
using GmtkJam2019.Entities.Core;

namespace GmtkJam2019.Entities.Enemies
{
	public abstract class Enemy : LivingEntity
	{
		private static EnemyData[] dataArray = JsonUtilities.Deserialize<EnemyData[]>("Enemies.json");

		private const float DamageTime = 0.25f;
		private const float DeathTime = 3;

		public static Player Player { get; set; }

		private Texture texture;
		private Sprite3D sprite;
		private EnemyData data;
		private SingleTimer tintTimer;
		private vec2 collisionBounds;
		private vec2 scale;
		private vec3 deathOrigin;

		// This means that the enemy was just hit and the sprite color is still transitioning back to white. If hit
		// again in this state, the existing timer is simply reset (rather than creating a new one).
		private bool damageActive;
		private bool isDying;

		private float deathRotation;
		private float fallRotation;

		protected Enemy(EnemyTypes type)
		{
			data = dataArray[(int)type];
			texture = ContentCache.GetTexture(data.Texture, true);

			if (data.Height == 0)
			{
				data.Height = (float)texture.Height / Sprite3D.PixelDivisor;
			}

			collisionBounds = new vec2(texture.Width, texture.Height) / Sprite3D.PixelDivisor;
			Health = data.Health;
			MaxHealth = Health;
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

		public EnemyData Data => data;

		public override void Initialize(Scene scene)
		{
			sprite = CreateSprite3D(scene, texture);
			sprite.Scale = scale;
			sprite.IsBillboarded = true;

			tintTimer = new SingleTimer(time => { damageActive = false; }, DamageTime);
			tintTimer.Paused = true;
			tintTimer.Repeatable = true;
			tintTimer.Tick = progress => {
				sprite.Color = Color.Lerp(Color.Red, Color.White, progress * progress);
			};

			components.Add(tintTimer);

			CreateBody(scene, new Circle(data.Radius), data.Height);

			base.Initialize(scene);
		}

		public override void ApplyDamage(int damage, vec3 direction, bool isHeadshot)
		{
			if (isHeadshot)
			{
				damage *= 2;
			}

			base.ApplyDamage(damage, direction, isHeadshot);

			if (Health == 0)
			{
				return;
			}

			if (damageActive)
			{
				tintTimer.Elapsed = 0;
			}
			else
			{
				tintTimer.Paused = false;
				sprite.Color = Color.Red;
				damageActive = true;
			}
		}

		protected override void OnDeath()
		{
			quat start = sprite.Orientation;
			quat end = start * quat.FromAxisAngle(-Constants.PiOverTwo, vec3.UnitX);

			var deathTimer = new SingleTimer(time => { IsMarkedForDestruction = true; }, DeathTime);
			deathTimer.Tick = progress =>
			{
				sprite.Orientation = quat.SLerp(start, end, progress);
				fallRotation = Utilities.Lerp(0, -Constants.PiOverTwo, progress);
			};

			components.Remove(tintTimer);
			components.Add(deathTimer);

			deathOrigin = position - new vec3(0, ControllingBody.Height / 2, 0);
			deathRotation = Rotation;
			ControllingBody.IsEnabled = false;
			sprite.Color = Color.Red;
			isDying = true;
		}

		public override void Update(float dt)
		{
			if (isDying)
			{
				//sprite.Position = deathOrigin + new vec3(0, data.Height / 2, 0) *
				//	(quat.FromAxisAngle(deathRotation, vec3.UnitY) * quat.FromAxisAngle(fallRotation, vec3.UnitX));
			}
			else
			{
				// This specific kind of billboarding works for a Doom-style FPS (where sprites only billboard along a
				// vertical axis).
				Rotation = Utilities.Angle(Position.swizzle.xz, Scene.Camera.Position.swizzle.xz);
				sprite.Orientation = quat.FromAxisAngle(-Rotation + Constants.PiOverTwo, vec3.UnitY);
			}

			base.Update(dt);
		}
	}
}
