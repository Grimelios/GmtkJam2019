using System.Collections.Generic;

namespace GmtkJam2019.Sensors
{
	public class HybridSpace
	{
		private List<HybridSensor> sensors;

		public HybridSpace()
		{
			sensors = new List<HybridSensor>();
		}

		public void Add(HybridSensor sensor)
		{
			sensors.Add(sensor);
		}

		public void Step()
		{
		}
	}
}
