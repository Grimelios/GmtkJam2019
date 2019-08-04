using GmtkJam2019.Entities.Core;

namespace GmtkJam2019.Entities.Enemies
{
	public class HealthBar : HybridEntity
	{
		public HealthBar()
		{
		}

		public int Health { get; set; }
		public int MaxHealth { get; set; }
	}
}
