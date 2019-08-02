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
			RunAcceleration = 180;
			RunMaxSpeed = 10;
		}

		public float RunAcceleration { get; }
		public float RunDeceleration { get; }
		public float RunMaxSpeed { get; }
	}
}
