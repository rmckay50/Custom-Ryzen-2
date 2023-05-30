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
	public class GetBarExample : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "GetBarExample";
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
			}
            else if (State == State.DataLoaded)
            {
                //clear the output window as soon as the bars data is loaded
                ClearOutputWindow();
            }
        }

		protected override void OnBarUpdate()
		{
            //	int barsAgo = CurrentBar - Bars.GetBar(new DateTime(2023, 05, 25, 9, 30, 0));
            //Print("\nbarsAgo: " + barsAgo.ToString());
            //// Check that its past 9:45 AM,
            ////if (ToTime(Time[0]) >= ToTime(7, 30, 00) && barsAgo > 0)
            //             if (ToTime(Time[0]) >= ToTime(9, 30, 00) )

            //             {
            //                 try
            //	{
            //		Print(String.Format("Time[0]: {0} ToTime(7, 30, 00): {1}", Time[0], ToTime(7, 30, 00)));
            //		// Calculate the bars ago value for the 9 AM bar for the current day
            //		//if (CurrentBar > barsAgo)
            //		//{
            //		// Print out the 7:30 AM bar closing price
            //		Print(String.Format("The close price on the 7:30 AM bar was: {0} Time: {1}", Close[barsAgo].ToString(), Time[0]));
            //		//}
            //	}
            //             catch (Exception ex)
            //             {
            //                 Print(String.Format("Time[0]: {0} ToTime(7, 30, 00): {1}", Time[0], ToTime(7, 30, 00)));
            //             }
            //         }

            //	GetBar() Example
            // Check that its past 9:45 AM
            if (ToTime(Time[0]) >= ToTime(9, 45, 00))
            {
                // Calculate the bars ago value for the 9 AM bar for the current day
                int barsAgo = CurrentBar - Bars.GetBar(new DateTime(2006, 12, 18, 9, 0, 0));

                // Print out the 9 AM bar closing price
                Print("The close price on the 9 AM bar was: " + Close[barsAgo].ToString());
            }
        }
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.GetBarExample[] cacheGetBarExample;
		public My.GetBarExample GetBarExample()
		{
			return GetBarExample(Input);
		}

		public My.GetBarExample GetBarExample(ISeries<double> input)
		{
			if (cacheGetBarExample != null)
				for (int idx = 0; idx < cacheGetBarExample.Length; idx++)
					if (cacheGetBarExample[idx] != null &&  cacheGetBarExample[idx].EqualsInput(input))
						return cacheGetBarExample[idx];
			return CacheIndicator<My.GetBarExample>(new My.GetBarExample(), input, ref cacheGetBarExample);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.GetBarExample GetBarExample()
		{
			return indicator.GetBarExample(Input);
		}

		public Indicators.My.GetBarExample GetBarExample(ISeries<double> input )
		{
			return indicator.GetBarExample(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.GetBarExample GetBarExample()
		{
			return indicator.GetBarExample(Input);
		}

		public Indicators.My.GetBarExample GetBarExample(ISeries<double> input )
		{
			return indicator.GetBarExample(input);
		}
	}
}

#endregion
