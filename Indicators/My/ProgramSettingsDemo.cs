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
using NinjaTrader.Custom.AddOns.Properties;
using System.Runtime;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.My
{
	public class ProgramSettingsDemo : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "ProgramSettingsDemo";
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
			else if ( State == State.DataLoaded ) 
			{
                ClearOutputWindow();
                var b = Settings.Default.temp;
                //var newB = Properties.Settings1.Date;
                Print(b);
                var c = DateTime.Now.ToString("MM/dd/yyyy");
                Settings.Default.temp = "04/04/2023";
                Settings.Default.Save();
                Print(Settings.Default.temp);
                Settings.Default.temp = "04/05/2023";
                Settings.Default.Save();
                Print(Settings.Default.temp);

                Print(Settings.Default.storedDate + "\n");
                Settings.Default.storedDate = "04/04/2023";
                Settings.Default.Save();

                Print(Settings.Default.storedDate + "\n");

                Settings.Default.storedDate = "04/05/2023";
                Settings.Default.Save();

                Print(Settings.Default.storedDate + "\n");
            }
        }

		protected override void OnBarUpdate()
		{


        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.ProgramSettingsDemo[] cacheProgramSettingsDemo;
		public My.ProgramSettingsDemo ProgramSettingsDemo()
		{
			return ProgramSettingsDemo(Input);
		}

		public My.ProgramSettingsDemo ProgramSettingsDemo(ISeries<double> input)
		{
			if (cacheProgramSettingsDemo != null)
				for (int idx = 0; idx < cacheProgramSettingsDemo.Length; idx++)
					if (cacheProgramSettingsDemo[idx] != null &&  cacheProgramSettingsDemo[idx].EqualsInput(input))
						return cacheProgramSettingsDemo[idx];
			return CacheIndicator<My.ProgramSettingsDemo>(new My.ProgramSettingsDemo(), input, ref cacheProgramSettingsDemo);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.ProgramSettingsDemo ProgramSettingsDemo()
		{
			return indicator.ProgramSettingsDemo(Input);
		}

		public Indicators.My.ProgramSettingsDemo ProgramSettingsDemo(ISeries<double> input )
		{
			return indicator.ProgramSettingsDemo(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.ProgramSettingsDemo ProgramSettingsDemo()
		{
			return indicator.ProgramSettingsDemo(Input);
		}

		public Indicators.My.ProgramSettingsDemo ProgramSettingsDemo(ISeries<double> input )
		{
			return indicator.ProgramSettingsDemo(input);
		}
	}
}

#endregion
