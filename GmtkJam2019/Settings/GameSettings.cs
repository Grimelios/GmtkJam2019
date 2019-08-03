using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmtkJam2019.Settings
{
	public class GameSettings
	{
		public GameSettings()
		{
			MouseSensitivity = 50;
		}

		public int MouseSensitivity { get; set; }

		public bool InvertX { get; set; }
		public bool InvertY { get; set; }
	}
}
