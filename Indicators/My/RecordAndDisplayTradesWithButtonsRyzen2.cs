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
	public class RecordAndDisplayTradesWithButtonsRyzen2 : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "RecordAndDisplayTradesWithButtonsRyzen2";
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
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.RecordAndDisplayTradesWithButtonsRyzen2[] cacheRecordAndDisplayTradesWithButtonsRyzen2;
		public My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2()
		{
			return RecordAndDisplayTradesWithButtonsRyzen2(Input);
		}

		public My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(ISeries<double> input)
		{
			if (cacheRecordAndDisplayTradesWithButtonsRyzen2 != null)
				for (int idx = 0; idx < cacheRecordAndDisplayTradesWithButtonsRyzen2.Length; idx++)
					if (cacheRecordAndDisplayTradesWithButtonsRyzen2[idx] != null &&  cacheRecordAndDisplayTradesWithButtonsRyzen2[idx].EqualsInput(input))
						return cacheRecordAndDisplayTradesWithButtonsRyzen2[idx];
			return CacheIndicator<My.RecordAndDisplayTradesWithButtonsRyzen2>(new My.RecordAndDisplayTradesWithButtonsRyzen2(), input, ref cacheRecordAndDisplayTradesWithButtonsRyzen2);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2()
		{
			return indicator.RecordAndDisplayTradesWithButtonsRyzen2(Input);
		}

		public Indicators.My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(ISeries<double> input )
		{
			return indicator.RecordAndDisplayTradesWithButtonsRyzen2(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2()
		{
			return indicator.RecordAndDisplayTradesWithButtonsRyzen2(Input);
		}

		public Indicators.My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(ISeries<double> input )
		{
			return indicator.RecordAndDisplayTradesWithButtonsRyzen2(input);
		}
	}
}

#endregion
