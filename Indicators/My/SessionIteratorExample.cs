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
namespace NinjaTrader.NinjaScript.Indicators.My
{
	public class SessionIteratorExample : Indicator
	{
        
		private SessionIterator sessionIterator;
		private bool firstPass = true;
        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "SessionIteratorExample";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
			}
			else if (State == State.Configure)
			{
                AddDataSeries(BarsPeriodType.Day, 1);
			}
            if (State == State.DataLoaded)
            {
				ClearOutputWindow();
				//stores the sessions once bars are ready, but before OnBarUpdate is called
				sessionIterator = new SessionIterator(Bars);
            }
        }

		protected override void OnBarUpdate()
		{
			
            // on new bars session, find the next trading session
            if (Bars.IsFirstBarOfSession)
            {
                Print("Calculating trading day for " + Time[0]);
                // use the current bar time to calculate the next session
                sessionIterator.GetNextSession(Time[0], true);

                // store the desired session information
                DateTime tradingDay = sessionIterator.ActualTradingDayExchange;
                DateTime beginTime = sessionIterator.ActualSessionBegin;
                DateTime endTime = sessionIterator.ActualSessionEnd;

                Print(string.Format("The Current Trading Day {0} starts at {1} and ends at {2}",
                                    tradingDay.ToShortDateString(), beginTime, endTime));
                Print(string.Format("The Current BarsArray[1].Count {0}", BarsArray[1].Count.ToString()));


                // Output:
                // Calculating trading day from 9/30/2015 4:01:00 PM
                //The Current Trading Day 10/1/2015 starts at 9/30/2015 4:00:00 PM and ends at 10/1/2015 3:00:00 PM
            }
			if (BarsArray[1].Count > 3)
			{
				if (firstPass)
				{
					//var b = BarsPeriod;
					//Print(string.Format("The Current BarsPeriod is {0}", b.ToString()));
					//var b1 = BarsArray[1].GetLow(1);
					Print(string.Format("BarsArray[1].GetLow(0); {0}", BarsArray[1].GetLow(0).ToString()));
                    Print(string.Format("BarsArray[1].GetLow(1); {0}", BarsArray[1].GetLow(1).ToString()));
                    Print(string.Format("BarsArray[1].GetLow(0); {0}", BarsArray[1].GetLow(2).ToString ()));
                    firstPass = false;
				}

            }




        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.SessionIteratorExample[] cacheSessionIteratorExample;
		public My.SessionIteratorExample SessionIteratorExample()
		{
			return SessionIteratorExample(Input);
		}

		public My.SessionIteratorExample SessionIteratorExample(ISeries<double> input)
		{
			if (cacheSessionIteratorExample != null)
				for (int idx = 0; idx < cacheSessionIteratorExample.Length; idx++)
					if (cacheSessionIteratorExample[idx] != null &&  cacheSessionIteratorExample[idx].EqualsInput(input))
						return cacheSessionIteratorExample[idx];
			return CacheIndicator<My.SessionIteratorExample>(new My.SessionIteratorExample(), input, ref cacheSessionIteratorExample);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.SessionIteratorExample SessionIteratorExample()
		{
			return indicator.SessionIteratorExample(Input);
		}

		public Indicators.My.SessionIteratorExample SessionIteratorExample(ISeries<double> input )
		{
			return indicator.SessionIteratorExample(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.SessionIteratorExample SessionIteratorExample()
		{
			return indicator.SessionIteratorExample(Input);
		}

		public Indicators.My.SessionIteratorExample SessionIteratorExample(ISeries<double> input )
		{
			return indicator.SessionIteratorExample(input);
		}
	}
}

#endregion
