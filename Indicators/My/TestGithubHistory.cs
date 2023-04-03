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
	public class TestGithubHistory : Indicator
	{
		public bool firstPass = true;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "TestGithubHistory";
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
			if (firstPass == true)
			{
				ClearOutputWindow();
				Print("first commit");
				firstPass = false;
			}
			//Add your custom indicator logic here.
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.TestGithubHistory[] cacheTestGithubHistory;
		public My.TestGithubHistory TestGithubHistory()
		{
			return TestGithubHistory(Input);
		}

		public My.TestGithubHistory TestGithubHistory(ISeries<double> input)
		{
			if (cacheTestGithubHistory != null)
				for (int idx = 0; idx < cacheTestGithubHistory.Length; idx++)
					if (cacheTestGithubHistory[idx] != null &&  cacheTestGithubHistory[idx].EqualsInput(input))
						return cacheTestGithubHistory[idx];
			return CacheIndicator<My.TestGithubHistory>(new My.TestGithubHistory(), input, ref cacheTestGithubHistory);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.TestGithubHistory TestGithubHistory()
		{
			return indicator.TestGithubHistory(Input);
		}

		public Indicators.My.TestGithubHistory TestGithubHistory(ISeries<double> input )
		{
			return indicator.TestGithubHistory(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.TestGithubHistory TestGithubHistory()
		{
			return indicator.TestGithubHistory(Input);
		}

		public Indicators.My.TestGithubHistory TestGithubHistory(ISeries<double> input )
		{
			return indicator.TestGithubHistory(input);
		}
	}
}

#endregion
