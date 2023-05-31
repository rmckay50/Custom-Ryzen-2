
#region
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
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

#region Comments
/*
 * https://forum.ninjatrader.com/forum/ninjatrader-8/indicator-development/1250603-plot-values
 * 
 * 2023 05 23 1245  
 *  commit - added VolumeIndicator for Draw.Text() example
 *  Draws number above or below bar. 
 *  Use for structure for print P/L on charts below trade line
 */
#endregion Comments
//This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators.VolumeIndicator
{
    public class VolumeIndicator : Indicator
    {
        private double longVolume;
        private double longVolumeAccumulated;
        private double shortVolume;
        private double shortVolumeAccumulated;
        private bool longTrend;
        private int BarstoDraw;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"My custom indicator";
                Name = "VolumeIndicator";
                Calculate = Calculate.OnBarClose;
                IsOverlay = true;
                DisplayInDataBox = true;
                DrawOnPricePanel = true;
                DrawHorizontalGridLines = false;
                DrawVerticalGridLines = false;
                PaintPriceMarkers = false;
                ScaleJustification = ScaleJustification.Right;
                //Disable this property if your indicator requires custom values that cumulate with each new market data event.
                //See Help Guide for additional information.
                IsSuspendedWhileInactive = true;
                BarstoDraw = CurrentBar;
            }
            else if (State == State.Configure)
            {
                AddPlot(Brushes.Red, "LongTrend");
                AddPlot(Brushes.Green, "ShortTrend");
                AddPlot(Brushes.Transparent, "TotalVolumeLong");
                AddPlot(Brushes.Transparent, "TotalVolumeShort");

            }
        }

        protected override void OnBarUpdate()
        {
			///*

            if (CurrentBar < 1000)
                return;

                    //if (Low[0] < Low[1] && High[0] == Close[0])
                    if ((High[0] == Close[0] && High[1] == Close[1]) || (High[0] > High[1] && High[1] > High[2] && Close[2] != Low[2]))
                    {
                        longTrend = true;
                        shortVolumeAccumulated = 0;
                        longVolume += Volume[0];
                    }
                    else if ((Low[0] == Close[0] && Low[1] == Close[1]) || (Low[0] < Low[1] && Low[1] < Low[2] && Close[2] != High[2]))
                    {
                        longTrend = false;
                        longVolumeAccumulated += longVolume;

                        shortVolume += Volume[0];
                    }

                    if (longTrend)
                    {
                        longVolumeAccumulated += Volume[0];
                        shortVolumeAccumulated = 0;
                        PlotBrushes[0][0] = Brushes.Transparent;
                        PlotBrushes[1][0] = Brushes.Transparent;
                    }
                    else
                    {
                        shortVolumeAccumulated += Volume[0];
                        longVolumeAccumulated = 0;
                        PlotBrushes[0][0] = Brushes.Transparent;
                        PlotBrushes[1][0] = Brushes.Transparent;
                    }

                    Values[0][0] = longTrend ? High[0] : double.NaN;
                    Values[1][0] = !longTrend ? Low[0] : double.NaN;
                    Values[2][0] = longVolumeAccumulated;
                    Values[3][0] = shortVolumeAccumulated;


                    if (longTrend)
                        {

                        Draw.Text(this, "L" + CurrentBar, "L " + longVolumeAccumulated, 0, High[0] + (5 * TickSize), Brushes.White);
                        }
                    else
                        {
                        Draw.Text(this, "S" + CurrentBar, "S " + shortVolumeAccumulated, 0, Low[0] - (5 * TickSize), Brushes.White);
                        }
            //Print(Time[0] + "   High " + Values[2][0] +  "           "  + Values[2][1] + "     " + CurrentBar);
			//*/
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private VolumeIndicator.VolumeIndicator[] cacheVolumeIndicator;
		public VolumeIndicator.VolumeIndicator VolumeIndicator()
		{
			return VolumeIndicator(Input);
		}

		public VolumeIndicator.VolumeIndicator VolumeIndicator(ISeries<double> input)
		{
			if (cacheVolumeIndicator != null)
				for (int idx = 0; idx < cacheVolumeIndicator.Length; idx++)
					if (cacheVolumeIndicator[idx] != null &&  cacheVolumeIndicator[idx].EqualsInput(input))
						return cacheVolumeIndicator[idx];
			return CacheIndicator<VolumeIndicator.VolumeIndicator>(new VolumeIndicator.VolumeIndicator(), input, ref cacheVolumeIndicator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.VolumeIndicator.VolumeIndicator VolumeIndicator()
		{
			return indicator.VolumeIndicator(Input);
		}

		public Indicators.VolumeIndicator.VolumeIndicator VolumeIndicator(ISeries<double> input )
		{
			return indicator.VolumeIndicator(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.VolumeIndicator.VolumeIndicator VolumeIndicator()
		{
			return indicator.VolumeIndicator(Input);
		}

		public Indicators.VolumeIndicator.VolumeIndicator VolumeIndicator(ISeries<double> input )
		{
			return indicator.VolumeIndicator(input);
		}
	}
}

#endregion
