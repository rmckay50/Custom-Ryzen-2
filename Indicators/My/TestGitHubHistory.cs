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
	public class TestGitHubHistory : Indicator
	{
		public bool firstPass = true;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Will change pint statements to output window and checkput different commits to see if it works";
				Name										= "TestGitHubHistory";
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
			if ( firstPass == true)
			{
				Print("first iteration");
				Print("second iteration");
				firstPass = false;
			}
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.TestGitHubHistory[] cacheTestGitHubHistory;
		public My.TestGitHubHistory TestGitHubHistory()
		{
			return TestGitHubHistory(Input);
		}

		public My.TestGitHubHistory TestGitHubHistory(ISeries<double> input)
		{
			if (cacheTestGitHubHistory != null)
				for (int idx = 0; idx < cacheTestGitHubHistory.Length; idx++)
					if (cacheTestGitHubHistory[idx] != null &&  cacheTestGitHubHistory[idx].EqualsInput(input))
						return cacheTestGitHubHistory[idx];
			return CacheIndicator<My.TestGitHubHistory>(new My.TestGitHubHistory(), input, ref cacheTestGitHubHistory);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.TestGitHubHistory TestGitHubHistory()
		{
			return indicator.TestGitHubHistory(Input);
		}

		public Indicators.My.TestGitHubHistory TestGitHubHistory(ISeries<double> input )
		{
			return indicator.TestGitHubHistory(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.TestGitHubHistory TestGitHubHistory()
		{
			return indicator.TestGitHubHistory(Input);
		}

		public Indicators.My.TestGitHubHistory TestGitHubHistory(ISeries<double> input )
		{
			return indicator.TestGitHubHistory(input);
		}
	}
}

#endregion
