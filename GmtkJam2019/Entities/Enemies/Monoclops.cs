using GlmSharp;

namespace GmtkJam2019.Entities.Enemies
{
	public class Monoclops : Enemy
	{
		public Monoclops() : base(EnemyTypes.Monoclops)
		{
			Scale = new vec2(2);
		}
	}
}
