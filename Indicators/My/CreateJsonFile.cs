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

using System.Text.Json;
using NinjaTrader.Custom.AddOns;
//using static NinjaTrader.NinjaScript.Indicators.My.RecordAndDisplayTradesWithButtons;
using System.IO;

#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.My
{
	public class CreateJsonFile : Indicator
	{
		private bool firstPass = true;
        List<ArrowLines> arrowLines = new List<ArrowLines>();
        private bool arrowLinesFirstPass = true;
        private bool arrowLinesSwitch = true;
        private Chart chartWindow;
        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "CreateJsonFile";
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
			if (firstPass)
			{
                hideArrowLines();
                firstPass = false;
            }
        }
        private void hideArrowLines()
        {
            ClearOutputWindow();



            foreach (DrawingTool dTL in DrawObjects.ToList())
            {
                //  change button color
                //  'break' kicks execution out of foreach
                if (dTL != null)
                {
                    //  find first arrowline
                    if (dTL.DisplayName == "Arrow line")
                    {

                        if (dTL.IsVisible)
                        {
                            arrowLinesSwitch = true;
                            //btnArrowLines.Background = Brushes.Green;

                            //  make invisible for debugging
                            //dTL.IsVisible = false;
                            ForceRefresh();
                            break;
                        }
                        else
                        {
                            arrowLinesSwitch = false;
                            //btnArrowLines.Background = Brushes.DimGray;
                            //  make visible for debugging
                            //dTL.IsVisible = true;
                            ForceRefresh();
                            break;
                        }
                    }
                }

            }

            //  clear arrowLines list
            arrowLines.Clear();

            //  Sets arrowLinesSwitch based on whether there are any drawings on the chart
            //  foreach (var obj in chartWindow.ActiveChartControl.ChartObjects)
            //  create list of arrowlines

            foreach (DrawingTool dTL in DrawObjects.ToList())
            {
                var anchors = dTL.Anchors.ToList();
                //var dTLTag = dTL.Tag;
                try
                {
                    //  is line an Arrow line?
                    if (dTL.DisplayName == "Arrow line")
                    {
                        var arrowLineId = dTL.Tag.Substring(11);
                        arrowLines.Add(new ArrowLines()
                        {
                            ID = arrowLineId,
                            StartTime = anchors[0].Time.ToString(),
                            StartY = (double)anchors[0].Price,
                            EndTime = anchors[1].Time.ToString(),
                            EndY = (double)anchors[1].Price
                        });
                    }
                    arrowLines = arrowLines.ToList();

                    //  set Serialize options
                    var options = new JsonSerializerOptions { WriteIndented = true };

                    //  Create Json string
                    string jsonString = JsonSerializer.Serialize(arrowLines, options);

                    //  Write to C:\data\ArrowLines.json
                    string fileName = @"C:\data\ArrowLines.json";

                    // Check to see if the file exists.
                    FileInfo fInfo = new FileInfo(fileName);

                    // You can throw a personalized exception if
                    // the file does not exist.
                    if (!fInfo.Exists)
                    {
                        throw new FileNotFoundException("The file was not found.", fileName);
                    }

                    //  Delete file contents
                    File.WriteAllText(fileName, String.Empty);

                    //  Write arrowList to file
                    File.WriteAllText(fileName, jsonString);
                    Print(jsonString);

                    var result = File.ReadAllText(fileName);
                    //Print(result);

                    //var jsonStringReturned = File.ReadAllText(fileName);
                    //Console.WriteLine("\njsonStringReturned\n" + jsonStringReturned);

                    ////var list1 = System.Text.Json.JsonSerializer.Deserialize<Person>(content);
                    //var arrowLinesList = System.Text.Json.JsonSerializer.Deserialize<List<ArrowLines>>(jsonStringReturned);
                }
                catch (Exception ex)
                {
                    Print(ex);
                }
                //  write arrowlines list to json file
                try
                {
                    if (arrowLines == null)
                    {
                        Print("Arrowlines == null");
                    }
                }
                catch (Exception ex)
                {
                    Print(ex);
                }
            }
            foreach (DrawingTool dTL in DrawObjects.ToList())
            {
                if (dTL.DisplayName == "Arrow line")
                {
                    if (arrowLinesSwitch)
                    {
                        //if (dTL.Tag.Contains("Text"))
                        if (dTL.DisplayName == "Arrow line")
                        {
                            Print(String.Format("Found arrowlinw {0}", dTL.Tag));
                        }
                        dTL.IsVisible = false;
                        ForceRefresh();
                    }
                    else if (!arrowLinesSwitch)
                    {
                        dTL.IsVisible = true;
                        ForceRefresh();
                    }
                }
            }

            //  write arrowlines list to Json file

            //*/
            arrowLinesSwitch = !arrowLinesSwitch;
            chartWindow.ActiveChartControl.InvalidateVisual();
            ForceRefresh();

        }

    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.CreateJsonFile[] cacheCreateJsonFile;
		public My.CreateJsonFile CreateJsonFile()
		{
			return CreateJsonFile(Input);
		}

		public My.CreateJsonFile CreateJsonFile(ISeries<double> input)
		{
			if (cacheCreateJsonFile != null)
				for (int idx = 0; idx < cacheCreateJsonFile.Length; idx++)
					if (cacheCreateJsonFile[idx] != null &&  cacheCreateJsonFile[idx].EqualsInput(input))
						return cacheCreateJsonFile[idx];
			return CacheIndicator<My.CreateJsonFile>(new My.CreateJsonFile(), input, ref cacheCreateJsonFile);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.CreateJsonFile CreateJsonFile()
		{
			return indicator.CreateJsonFile(Input);
		}

		public Indicators.My.CreateJsonFile CreateJsonFile(ISeries<double> input )
		{
			return indicator.CreateJsonFile(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.CreateJsonFile CreateJsonFile()
		{
			return indicator.CreateJsonFile(Input);
		}

		public Indicators.My.CreateJsonFile CreateJsonFile(ISeries<double> input )
		{
			return indicator.CreateJsonFile(input);
		}
	}
}

#endregion
