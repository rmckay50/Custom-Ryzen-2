#region Comments
/*
 *	2022 11 18 1535
 *		Use with TryCatchClass in NinjaTrader.Custom.AddOns
 *		Writes Exception to C:\Error.txt
 *		Need to include 'NinjaTrader.Custom.AddOns'
 * 
 */
#endregion Comments

#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
using NinjaTrader.Custom.AddOns;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.My
{
	public class TryCatchExample : Indicator
	{
		
		protected override void OnStateChange()
		{
			try
			{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "TryCatchExample";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				
//				int[] ara = {1,2};
//				int x = ara[3];

			}
			else if (State == State.Configure)
			{
				int[] ara = {1,2};
				int x = ara[3];
			}
			}
			catch(Exception ex)
			{
				Print(string.Format("in catch"));string filePath = @"C:\Error.txt";
                TryCatchClass.TryCatch(ex);

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
		private My.TryCatchExample[] cacheTryCatchExample;
		public My.TryCatchExample TryCatchExample()
		{
			return TryCatchExample(Input);
		}

		public My.TryCatchExample TryCatchExample(ISeries<double> input)
		{
			if (cacheTryCatchExample != null)
				for (int idx = 0; idx < cacheTryCatchExample.Length; idx++)
					if (cacheTryCatchExample[idx] != null &&  cacheTryCatchExample[idx].EqualsInput(input))
						return cacheTryCatchExample[idx];
			return CacheIndicator<My.TryCatchExample>(new My.TryCatchExample(), input, ref cacheTryCatchExample);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.TryCatchExample TryCatchExample()
		{
			return indicator.TryCatchExample(Input);
		}

		public Indicators.My.TryCatchExample TryCatchExample(ISeries<double> input )
		{
			return indicator.TryCatchExample(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.TryCatchExample TryCatchExample()
		{
			return indicator.TryCatchExample(Input);
		}

		public Indicators.My.TryCatchExample TryCatchExample(ISeries<double> input )
		{
			return indicator.TryCatchExample(input);
		}
	}
}

#endregion
