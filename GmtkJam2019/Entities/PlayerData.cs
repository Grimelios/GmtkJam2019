using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmtkJam2019.Entities
{
	public class PlayerData
	{
		public PlayerData()
		{
			RunAcceleration = 200;
			RunDeceleration = 180;
			RunMaxSpeed = 10;
			ViewOffset = 0.7f;
		}

		public float RunAcceleration { get; }
		public float RunDeceleration { get; }
		public float RunMaxSpeed { get; }
		public float ViewOffset { get; }
	}
}
