//
// Copyright (C) 2015, NinjaTrader LLC <www.ninjatrader.com>
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
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
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class TxBIPxSample : Indicator
	{ 
		
			private TimeSpan endTime = new TimeSpan(16,15,00);
            private int startHour = 9; // Default setting for StartHour
            private int startMinute = 30; // Default setting for StartMinute
            private int endHour = 16; // Default setting for EndHour
            private int endMinute = 15; // Default setting for EndMinute
			private double rthMid = 0;
			private double rthClose = 0;
			private int iRegionOpacity 	= 5;
		
			private double rangeopenprice = 0;
		
		protected override void OnStateChange()
		{
			
			if (State == State.SetDefaults)
			{
				Description					= @"Determines the highest high and lowest low in a specified time range";
				Name						= "TxBIPxSample";
				Calculate					= Calculate.OnBarClose;
				IsOverlay					= true;
				DisplayInDataBox			= true;
				DrawOnPricePanel			= true;
				DrawHorizontalGridLines		= true;
				DrawVerticalGridLines		= true;
				PaintPriceMarkers			= true;
				ScaleJustification 			= NinjaTrader.Gui.Chart.ScaleJustification.Right;
//				StartHour					= 9;
//				StartMinute					= 30;
//				EndHour						= 10;
//				EndMinute					= 15;
				IsSuspendedWhileInactive					= true;
//				AddPlot(Brushes.Green, "HighestHigh");
//				AddPlot(Brushes.Red, "LowestLow");
			}
			else if (State == State.Configure)
			{
			AddDataSeries(BarsPeriodType.Minute, 15);	
			}
		}
		
		private DateTime startDateTime;
		private DateTime endDateTime;
		protected override void OnBarUpdate()
		{
			if (State != State.Realtime)
				return;
			if (CurrentBars[0]<1 || CurrentBars[1]<1)
				return;

			if (CurrentBar < 1)
				return;

			// Check to make sure the end time is not earlier than the start time
			if (EndHour < StartHour)
			    return;
//			if(CurrentBar<10) return;
			
			//Do not calculate the high or low value when the ending time of the desired range is less than the current time of the bar being processed
			if (ToTime(EndHour,EndMinute,0) > ToTime(Time[0]))
			    return;   
	
			if(BarsInProgress==1)

			{	
			bool isTimeEnd = Time[0].TimeOfDay.CompareTo(endTime)>=0 && (Time[1].TimeOfDay.CompareTo(endTime)<0 || Time[0].Date!=Time[1].Date);

			if (startDateTime.Date != Time[0].Date)
			{
			    startDateTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, StartHour, StartMinute, 0);
			    endDateTime = new DateTime(Time[0].Year, Time[0].Month, Time[0].Day, EndHour, EndMinute, 0);	
			}

			// Calculate the number of bars ago for the start and end bars of the specified time range
			int startBarsAgo = BarsArray[1].GetBar(startDateTime);
			int endBarsAgo = BarsArray[1].GetBar(endDateTime);
			
				
			double highestHigh = MAX(Highs[1], endBarsAgo - startBarsAgo  + 1)[CurrentBars[1] - endBarsAgo];
			 
			// Now that we have the start and end bars ago values for the specified time range we can calculate the lowest low for this range
			double lowestLow = MIN(Lows[1], endBarsAgo - startBarsAgo + 1)[CurrentBars[1] - endBarsAgo];
			
			// uU hier gleich alles! einziger Unterschied dass 2 Rectangles und EU-session nicht nur Blau! Idee blauer Rahmen, aber den dann mit eigener Logik damit nur am aktuellen Tag!
			if (isTimeEnd)
			{
			Draw.Rectangle(this, "up" + CurrentBar, false, CurrentBars[1] - startBarsAgo, highestHigh, CurrentBars[1] - endBarsAgo, lowestLow, Brushes.Transparent, Brushes.LightGreen, iRegionOpacity);
			
			}
			}
		}
		

		#region Properties
		
		[Description("Opacity for painted region")]
//        [GridCategory("Parameters")]
        public int _RegionOpacity
        {
            get { return iRegionOpacity; }
//			set { iRegionOpacity = Math.Min(Math.Max(0, value),10); }
            set { iRegionOpacity = value; }
        }
		
//		[Browsable(false)]
//		[XmlIgnore]
//		public Series<double> HighestHigh
//		{
//			get { return Values[0]; }
//		}

//		[Browsable(false)]
//		[XmlIgnore]
//		public Series<double> LowestLow
//		{
//			get { return Values[1]; }
//		}
		
		
//		[Browsable(false)]
//		[XmlIgnore]
//		public Series<double> HighestHigh
//		{
//			get { return Values[0]; }
//		}

//		[Browsable(false)]
//		[XmlIgnore]
//		public Series<double> LowestLow
//		{
//			get { return Values[1]; }
//		}
		
		
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> TMid
		{
			get { return Values[1]; }
		}
		
		[Range(0,23)]
		[NinjaScriptProperty]
		[Display(Name="Start hour", Description = "Enter start hour, Military time format 0 - 23", Order=1, GroupName="Parameters")]
		public int StartHour
		{ get; set; }

		[Range(0, 59)]
		[NinjaScriptProperty]
		[Display(Name="Start minute", Description = "Enter start minute(s) 0 - 59",Order=2, GroupName="Parameters")]
		public int StartMinute
		{ get; set; }

		[Range(0, 23)]
		[NinjaScriptProperty]
		[Display(Name="End hour", Description = "Enter end hour, Military time format 0 - 23",Order=3, GroupName="Parameters")]
		public int EndHour
		{ get; set; }

		[Range(0, 59)]
		[NinjaScriptProperty]
		[Display(Name="End minute",Description = " Enter end minute(s) 0 - 59", Order=4, GroupName="Parameters")]
		public int EndMinute
		{ get; set; }
		
		[Description("")]
//		[GridCategory("Parameters")]		
		[XmlIgnore()]
        public TimeSpan EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }
		
		[Browsable(false)]
		public string EndTimeS
		{
			get { return endTime.ToString(); }
 			set { endTime = TimeSpan.Parse(value); }
		}

		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private TxBIPxSample[] cacheTxBIPxSample;
		public TxBIPxSample TxBIPxSample(int startHour, int startMinute, int endHour, int endMinute)
		{
			return TxBIPxSample(Input, startHour, startMinute, endHour, endMinute);
		}

		public TxBIPxSample TxBIPxSample(ISeries<double> input, int startHour, int startMinute, int endHour, int endMinute)
		{
			if (cacheTxBIPxSample != null)
				for (int idx = 0; idx < cacheTxBIPxSample.Length; idx++)
					if (cacheTxBIPxSample[idx] != null && cacheTxBIPxSample[idx].StartHour == startHour && cacheTxBIPxSample[idx].StartMinute == startMinute && cacheTxBIPxSample[idx].EndHour == endHour && cacheTxBIPxSample[idx].EndMinute == endMinute && cacheTxBIPxSample[idx].EqualsInput(input))
						return cacheTxBIPxSample[idx];
			return CacheIndicator<TxBIPxSample>(new TxBIPxSample(){ StartHour = startHour, StartMinute = startMinute, EndHour = endHour, EndMinute = endMinute }, input, ref cacheTxBIPxSample);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.TxBIPxSample TxBIPxSample(int startHour, int startMinute, int endHour, int endMinute)
		{
			return indicator.TxBIPxSample(Input, startHour, startMinute, endHour, endMinute);
		}

		public Indicators.TxBIPxSample TxBIPxSample(ISeries<double> input , int startHour, int startMinute, int endHour, int endMinute)
		{
			return indicator.TxBIPxSample(input, startHour, startMinute, endHour, endMinute);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.TxBIPxSample TxBIPxSample(int startHour, int startMinute, int endHour, int endMinute)
		{
			return indicator.TxBIPxSample(Input, startHour, startMinute, endHour, endMinute);
		}

		public Indicators.TxBIPxSample TxBIPxSample(ISeries<double> input , int startHour, int startMinute, int endHour, int endMinute)
		{
			return indicator.TxBIPxSample(input, startHour, startMinute, endHour, endMinute);
		}
	}
}

#endregion
