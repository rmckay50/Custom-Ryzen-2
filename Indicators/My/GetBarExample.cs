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
using SharpDX.Direct2D1;
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
				ScaleJustification							= ScaleJustification.Right;
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
            DateTime StartTime = DateTime.Parse("08:54:05  05/25/2023");
            DateTime EndTime = DateTime.Parse("09:26:12  05/25/2023");
            Draw.Line
                (this,
                "First Line",
                false,
                StartTime,
                183.94,
                DateTime.Parse(EndTime.ToString()),
                184.7,
                Brushes.Blue,
                DashStyleHelper.Solid,
                5);

            //var sTime = DateTime.Parse(rc.StartTime);
            int barsAgo = CurrentBar - Bars.GetBar(StartTime);
            if (barsAgo > 0)
            {
                var _textYStartingPoint = Low[barsAgo];

                Draw.Text(this, CurrentBar.ToString() + "P/L", "6.7", barsAgo, Low[barsAgo]);

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
