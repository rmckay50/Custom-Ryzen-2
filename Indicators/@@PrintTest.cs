#region Comments
/*
 * 2023 02 19 1730 
 *	example of putting print statement in method
 * 
*/
#endregion Comments

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
using System.Diagnostics;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.My
{
	public class Printer : Indicator
	{
		private bool firstPass;
		private string userName = Environment.UserName;
//		private string inputFile = @"C:\Users\" + userName + @"\Documents\NinjaTrader 8\db\NinjaTrader.sqlite";
			protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Print on each pass to check output window";
				Name										= "Print";
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
				
//				InputFile = @"C:\Users\Owner\Documents\NinjaTrader 8\db\NinjaTrader.sqlite";
				InputFile = @"C:\Users\" + userName + @"\Documents\NinjaTrader 8\db\NinjaTrader.sqlite";
                OutputFile = @"C:\Users\" + userName + @"\Documents\NinjaTrader 8\csvNTDrawline.csv";

			}
			else if (State == State.Configure)
			{
			}

			else if (State == State.Historical)
            {
//				for (int i = 0; i < 10; i++) 
//				{
//					if (i > 5)
//					{
//						Debug.Indent(); ;
//						Debug.WriteLine("In Writeline");
////						Debug.Assert(false, "breakpoint if > 6");
//						//throw new Exception();
//					}
//					Print(string.Format("printed to output {0}", i));
					Print(string.Format("MachineName: {0}", Environment.CurrentDirectory));
					Print(string.Format("MachineName: {0}", Environment.MachineName));
					Print(string.Format("MachineName: {0}", Environment.UserName));
					Print(string.Format("InputFile: {0}", InputFile));

			
//				}
            }
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
		}
		
		        [NinjaScriptProperty]
        [Display(Name = "InputFile", Order = 4, GroupName = "Parameters")]
        [PropertyEditor("NinjaTrader.Gui.Tools.FilePathPicker", Filter = "Any Files (*.csv)|*.*")]
        //public string MyFile
        public string InputFile
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "OutputFile", Order = 5, GroupName = "Parameters")]
        [PropertyEditor("NinjaTrader.Gui.Tools.FilePathPicker", Filter = "Any Files (*.csv)|*.*")]
        //public string MyFile
        public string OutputFile
        { get; set; }

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.Printer[] cachePrinter;
		public My.Printer Printer(string inputFile, string outputFile)
		{
			return Printer(Input, inputFile, outputFile);
		}

		public My.Printer Printer(ISeries<double> input, string inputFile, string outputFile)
		{
			if (cachePrinter != null)
				for (int idx = 0; idx < cachePrinter.Length; idx++)
					if (cachePrinter[idx] != null && cachePrinter[idx].InputFile == inputFile && cachePrinter[idx].OutputFile == outputFile && cachePrinter[idx].EqualsInput(input))
						return cachePrinter[idx];
			return CacheIndicator<My.Printer>(new My.Printer(){ InputFile = inputFile, OutputFile = outputFile }, input, ref cachePrinter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.Printer Printer(string inputFile, string outputFile)
		{
			return indicator.Printer(Input, inputFile, outputFile);
		}

		public Indicators.My.Printer Printer(ISeries<double> input , string inputFile, string outputFile)
		{
			return indicator.Printer(input, inputFile, outputFile);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.Printer Printer(string inputFile, string outputFile)
		{
			return indicator.Printer(Input, inputFile, outputFile);
		}

		public Indicators.My.Printer Printer(ISeries<double> input , string inputFile, string outputFile)
		{
			return indicator.Printer(input, inputFile, outputFile);
		}
	}
}

#endregion
