using GmtkJam2019.Entities.Core;

namespace GmtkJam2019.Entities.Objects
{
	public class Door : HybridEntity
	{
		public bool IsLocked { get; set; }

		public override void Initialize(Scene scene)
		{
			base.Initialize(scene);
		}
	}
}
