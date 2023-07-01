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
	public class SessionIterator : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "SessionIterator";
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
		private My.SessionIterator[] cacheSessionIterator;
		public My.SessionIterator SessionIterator()
		{
			return SessionIterator(Input);
		}

		public My.SessionIterator SessionIterator(ISeries<double> input)
		{
			if (cacheSessionIterator != null)
				for (int idx = 0; idx < cacheSessionIterator.Length; idx++)
					if (cacheSessionIterator[idx] != null &&  cacheSessionIterator[idx].EqualsInput(input))
						return cacheSessionIterator[idx];
			return CacheIndicator<My.SessionIterator>(new My.SessionIterator(), input, ref cacheSessionIterator);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.SessionIterator SessionIterator()
		{
			return indicator.SessionIterator(Input);
		}

		public Indicators.My.SessionIterator SessionIterator(ISeries<double> input )
		{
			return indicator.SessionIterator(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.SessionIterator SessionIterator()
		{
			return indicator.SessionIterator(Input);
		}

		public Indicators.My.SessionIterator SessionIterator(ISeries<double> input )
		{
			return indicator.SessionIterator(input);
		}
	}
}

#endregion
