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
        private int _barCntr = 1;
        private int barNumber;
		private DateTime startTime = DateTime.Parse("08:54:05  05/25/2023");
		private bool firstPass = true;
        private bool _textIsBelowBars = true;
        private int _fontSize = 14;
        private int _pixelsAboveBelowBar = -50;
        private Brush _textColorDefinedbyUser = Brushes.Gray;
        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description = @"Attempting to get number of bar using GetBar().";
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
            else if (State == State.DataLoaded)
            {
                //clear the output window as soon as the bars data is loaded
                ClearOutputWindow();
            }
        }

		protected override void OnBarUpdate()
		{
			var st = DateTime.Parse("05/26/2023 07:40:00");
		if (CurrentBar > 1)
			{
				Print(String.Format("\nCurrentBar: {0} Close[0]: {1} Close[1]: {2} ToTime(Time[0]): {3} ToTime(st): {4}", CurrentBar, Close[0], Close[1], ToTime(Time[0]), ToTime(st)));
			}

			double _textYStartingPoint = High[0]; //position text above or below bars
			if (_textIsBelowBars)
				_textYStartingPoint = Low[0];
			NinjaTrader.Gui.Tools.SimpleFont myFont = new NinjaTrader.Gui.Tools.SimpleFont("Courier New", 14) { Size = _fontSize, Bold = false };
					//	Text to print
					var pL = 6.20;
			if (firstPass == true)
			{
				Draw.Text(this, CurrentBar.ToString(), true, pL.ToString(), st, _textYStartingPoint, _pixelsAboveBelowBar, _textColorDefinedbyUser, myFont, TextAlignment.Center, null, null, 1);
				firstPass= false;
			}
			if (CurrentBar < 5)
			{
				Print("Bar number: " + CurrentBar.ToString());
				Print("Bar time: " + Time[0].ToString() + "Bar Close: " + Close[0]);
				// Check that its past 9:45 AM
				//if (ToTime(Time[0]) == ToTime(7, 35, 00))
				if (ToTime(Time[0]) >= ToTime(st))

				{
					//	Bars.GetBar is working
					int barsAgo = Bars.GetBar(st);
                    Draw.Text(this, CurrentBar.ToString(), true, pL.ToString(), 0, _textYStartingPoint, _pixelsAboveBelowBar, _textColorDefinedbyUser, myFont, TextAlignment.Center, null, null, 1);

                    if (CurrentBar > barsAgo)
					{
						Print("barsAgo:" + barsAgo.ToString());
						// Print out the 9 AM bar closing price
						//Print("*** The close price on the 7:35 AM bar was: " + Close[barsAgo].ToString());
						//Draw.Text(this, CurrentBar.ToString(), true, _barCntr.ToString(), 0, _textYStartingPoint, _pixelsAboveBelowBar, _textColorDefinedbyUser, myFont, TextAlignment.Center, null, null, 1);
						Draw.Text(this, CurrentBar.ToString(), true, pL.ToString(), 0, _textYStartingPoint, _pixelsAboveBelowBar, _textColorDefinedbyUser, myFont, TextAlignment.Center, null, null, 1);

                        //Print("The close price on the 9 AM bar was: " + Close[0].ToString());
                    }
                }
			}
			//if (CurrentBar > 0)
			//{
			//	try
			//	{
			//		int barsAgo = CurrentBar - Bars.GetBar(startTime);
			//		Print(String.Format("CurrentBar is {0} barsAgo: {1}", CurrentBar.ToString(), barsAgo.ToString()));
			//		// Print out the 9 AM bar closing price
			//		Print("The close price on the 9 AM bar was: " + Close[barsAgo].ToString());
			//		firstPass = false;
			//	}
			//	catch (Exception ex)
			//	{
			//		Console.WriteLine(ex.ToString());
			//	}
			//	////}
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
