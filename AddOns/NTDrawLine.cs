using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.Custom.AddOns
{

	#region class NTDrawLine
	public class NTDrawLine
	{
		//DateTime startTime, double startY, DateTime endTime, double endY
		public int Id { get; set; }
        public string Name { get; set; }
        public string Long_Short { get; set; }
		public long StartTimeTicks { get; set; }
		public string StartTime { get; set; }
		public double StartY { get; set; }
		public long EndTimeTicks { get; set; }
		public string EndTime { get; set; }
		public double EndY { get; set; }
		public double P_L { get; set; }

		public NTDrawLine() { }

		public NTDrawLine(
			int id, 
			string name, 
			string long_Short, 
			long startTimeTicks, 
			string startTime, 
			double startY, 
			long endTimeTicks, 
			string endTime, 
			double endY,
			double p_L)
		{
			Id = id;
			Name = name;
			Long_Short = long_Short;
			StartTimeTicks = startTimeTicks;
			StartTime = startTime;
			StartY = startY;
			EndTimeTicks = endTimeTicks;
			EndTime = endTime;
			EndY = endY;
			P_L = p_L;
		}
	}
	#endregion

}


