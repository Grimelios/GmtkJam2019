using Engine.Shapes._2D;
using Engine.Shapes._3D;
using Engine.View;
using GlmSharp;
using GmtkJam2019.Entities.Core;
using GmtkJam2019.Physics;
using GmtkJam2019.UI;
using GmtkJam2019.Weapons;

namespace GmtkJam2019.Entities
{
	public class Player : LivingEntity
	{
		private Camera3D camera;
		private PlayerData playerData;
		private PlayerVisionDisplay visionDisplay;

		public Player(Camera3D camera)
		{
			this.camera = camera;

			playerData = new PlayerData();
			Controller = new PlayerController(this, playerData, camera);
			MaxHealth = 3;
			Health = MaxHealth;
		}

		public PlayerController Controller { get; }
		public PlayerVisionDisplay VisionDisplay
		{
			set
			{
				visionDisplay = value;
				visionDisplay.Health = Health;
			}
		}

		public Weapon Weapon { get; private set; }

		public override vec3 Position
		{
			get => base.Position;
			set
			{
				camera.Position = position + new vec3(0, playerData.ViewOffset, 0);

				if (Weapon != null)
				{
					Weapon.Position = value;
				}

				base.Position = value;
			}
		}

		public vec3 Eye => Position + new vec3(0, playerData.ViewOffset, 0);
		
		public Line3D ShotLine { get; set; }
		public Line3D NormalLine { get; set; }

		public override void Initialize(Scene scene)
		{
			CreateBody(scene, new Circle(0.5f), 2);

			base.Initialize(scene);
		}

		public override void ApplyDamage(int damage, vec3 direction)
		{
			base.ApplyDamage(damage, direction);

			visionDisplay.Health = Health;
		}

		public void RemoveEye()
		{
			visionDisplay.RemoveEye();
			camera.IsOrthographic = true;
		}

		public void Equip(Weapon weapon)
		{
			Weapon = weapon;
			weapon.Owner = this;
		}
	}
}
