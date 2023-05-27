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
	public class GetBarNumber : Indicator
	{
		private int barNumber;
		private DateTime startTime = DateTime.Parse("08:54:05  05/25/2023");
		private bool firstPass = true;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description = @"Enter the description for your new custom Indicator here.";
				Name = "GetBarNumber";
				Calculate = Calculate.OnBarClose;
				IsOverlay = false;
				DisplayInDataBox = true;
				DrawOnPricePanel = true;
				DrawHorizontalGridLines = true;
				DrawVerticalGridLines = true;
				PaintPriceMarkers = true;
				ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive = true;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.Historical)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			//// Check that its past 9:45 AM
			//if (ToTime(Time[0]) >= ToTime(9, 45, 00))
			//{
			//    // Calculate the bars ago value for the 9 AM bar for the current day
			//    int barsAgo = Bars.GetBar(new DateTime(2023, 05, 26, 7, 30, 0));

			//    // Print out the 9 AM bar closing price
			//    Print("The close price on the 9 AM bar was: " + Close[barsAgo].ToString());
			//}
			//if (CurrentBar > 0)
			//{
				try
				{
					int barsAgo = CurrentBar - Bars.GetBar(startTime);
					Print(String.Format("CurrentBar is {0} {1}", CurrentBar.ToString(), barsAgo.ToString()));
					// Print out the 9 AM bar closing price
					Print("The close price on the 9 AM bar was: " + Close[barsAgo].ToString());
					firstPass = false;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			//}
		}

    }
}


#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.GetBarNumber[] cacheGetBarNumber;
		public My.GetBarNumber GetBarNumber()
		{
			return GetBarNumber(Input);
		}

		public My.GetBarNumber GetBarNumber(ISeries<double> input)
		{
			if (cacheGetBarNumber != null)
				for (int idx = 0; idx < cacheGetBarNumber.Length; idx++)
					if (cacheGetBarNumber[idx] != null &&  cacheGetBarNumber[idx].EqualsInput(input))
						return cacheGetBarNumber[idx];
			return CacheIndicator<My.GetBarNumber>(new My.GetBarNumber(), input, ref cacheGetBarNumber);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.GetBarNumber GetBarNumber()
		{
			return indicator.GetBarNumber(Input);
		}

		public Indicators.My.GetBarNumber GetBarNumber(ISeries<double> input )
		{
			return indicator.GetBarNumber(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.GetBarNumber GetBarNumber()
		{
			return indicator.GetBarNumber(Input);
		}

		public Indicators.My.GetBarNumber GetBarNumber(ISeries<double> input )
		{
			return indicator.GetBarNumber(input);
		}
	}
}

#endregion
