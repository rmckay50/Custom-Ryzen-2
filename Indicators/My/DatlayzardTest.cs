#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class DatlayzardTest : Indicator
	{
		private SessionIterator	sessionIterator;
		private long			priorSessionVolume;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Name						= "DatlayzardTest";
				Calculate					= Calculate.OnBarClose;
				IsOverlay					= false;
			}
			else if (State == State.DataLoaded)
			{
				sessionIterator = new SessionIterator(Bars);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 1 || (State == State.Historical && CurrentBar < Count - 10) )
				return;

			if (Bars.IsFirstBarOfSession)
			{
				sessionIterator.GetNextSession(Time[0], true);
				sessionIterator.GetNextSession(Bars.GetTime(Bars.GetBar(sessionIterator.ActualSessionBegin) - 1), true);
				//Print(string.Format("Current bar: {0} | Get next session -1 bar actual begin bar: {1}", Time[0], sessionIterator.ActualSessionBegin));
			}

			int lastSessionStartIndex = Bars.GetBar(sessionIterator.ActualSessionBegin);
			for (int i = lastSessionStartIndex; i < CurrentBar; i++)
			{
				priorSessionVolume += Bars.GetVolume(i);
			}

			Print(string.Format("{0} | priorSessionVolume: {1}", Time[0], priorSessionVolume));
			priorSessionVolume = 0;
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private DatlayzardTest[] cacheDatlayzardTest;
		public DatlayzardTest DatlayzardTest()
		{
			return DatlayzardTest(Input);
		}

		public DatlayzardTest DatlayzardTest(ISeries<double> input)
		{
			if (cacheDatlayzardTest != null)
				for (int idx = 0; idx < cacheDatlayzardTest.Length; idx++)
					if (cacheDatlayzardTest[idx] != null &&  cacheDatlayzardTest[idx].EqualsInput(input))
						return cacheDatlayzardTest[idx];
			return CacheIndicator<DatlayzardTest>(new DatlayzardTest(), input, ref cacheDatlayzardTest);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.DatlayzardTest DatlayzardTest()
		{
			return indicator.DatlayzardTest(Input);
		}

		public Indicators.DatlayzardTest DatlayzardTest(ISeries<double> input )
		{
			return indicator.DatlayzardTest(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.DatlayzardTest DatlayzardTest()
		{
			return indicator.DatlayzardTest(Input);
		}

		public Indicators.DatlayzardTest DatlayzardTest(ISeries<double> input )
		{
			return indicator.DatlayzardTest(input);
		}
	}
}

#endregion
