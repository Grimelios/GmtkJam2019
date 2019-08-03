using Engine.Shapes._2D;
using Engine.View;
using GlmSharp;
using GmtkJam2019.Entities.Core;
using GmtkJam2019.Physics;
using GmtkJam2019.UI;

namespace GmtkJam2019.Entities
{
	public class Player : LivingEntity
	{
		private Camera3D camera;
		private PlayerData playerData;
		private PlayerController controller;

		public Player(Camera3D camera)
		{
			this.camera = camera;

			playerData = new PlayerData();
			controller = new PlayerController(this, playerData, camera);
			Body = new HybridBody(new Circle(0.5f), 2, this);
			MaxHealth = 3;
			Health = MaxHealth;
		}

		public PlayerVisionDisplay VisionDisplay { get; set; }

		public override void ApplyDamage(int damage)
		{
			base.ApplyDamage(damage);

			VisionDisplay.Health = Health;
		}

		public override void Update(float dt)
		{
			camera.Position = position + new vec3(0, playerData.ViewOffset, 0);

			base.Update(dt);
		}
	}
}
