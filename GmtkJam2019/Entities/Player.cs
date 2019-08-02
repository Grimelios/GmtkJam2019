using Engine.Shapes._2D;
using GmtkJam2019.Entities.Core;
using GmtkJam2019.Physics;
using GmtkJam2019.UI;

namespace GmtkJam2019.Entities
{
	public class Player : HybridEntity
	{
		private PlayerData playerData;
		private PlayerController controller;

		public Player()
		{
			playerData = new PlayerData();
			controller = new PlayerController(this, playerData);
			Body = new HybridBody(new Circle(0.5f), 2, this);
		}

		public PlayerVisionDisplay VisionDisplay { get; set; }
	}
}
