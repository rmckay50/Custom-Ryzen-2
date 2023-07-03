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
	public class AddDataSeries : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "AddDataSeries";
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
                AddDataSeries(BarsPeriodType.Day, 1);
            }
		}

		protected override void OnBarUpdate()
		{
            #region Create dictionary
            //  create dictionary of daily closes to be used with placing text
            IDictionary<string, double> dictDayClose = new Dictionary<string, double>();

            //  dictionary is created in method
            dictDayClose = DictDayClose();
            #endregion Create dictionary

        }
        public IDictionary<string, double> DictDayClose()
        {
            IDictionary<string, double> dictDayClose = new Dictionary<string, double>();
            for (int i = 0; i < BarsArray[1].Count; i++)
            {
				var z = Times[1][i];
                var x = BarsArray[1].GetTime(i).ToString("MM/dd/yyyy");
                var y = BarsArray[1].GetLow(i);
                dictDayClose.Add(x, y);

            }

            return dictDayClose;
        }


    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.AddDataSeries[] cacheAddDataSeries;
		public My.AddDataSeries AddDataSeries()
		{
			return AddDataSeries(Input);
		}

		public My.AddDataSeries AddDataSeries(ISeries<double> input)
		{
			if (cacheAddDataSeries != null)
				for (int idx = 0; idx < cacheAddDataSeries.Length; idx++)
					if (cacheAddDataSeries[idx] != null &&  cacheAddDataSeries[idx].EqualsInput(input))
						return cacheAddDataSeries[idx];
			return CacheIndicator<My.AddDataSeries>(new My.AddDataSeries(), input, ref cacheAddDataSeries);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.AddDataSeries AddDataSeries()
		{
			return indicator.AddDataSeries(Input);
		}

		public Indicators.My.AddDataSeries AddDataSeries(ISeries<double> input )
		{
			return indicator.AddDataSeries(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.AddDataSeries AddDataSeries()
		{
			return indicator.AddDataSeries(Input);
		}

		public Indicators.My.AddDataSeries AddDataSeries(ISeries<double> input )
		{
			return indicator.AddDataSeries(input);
		}
	}
}

#endregion
