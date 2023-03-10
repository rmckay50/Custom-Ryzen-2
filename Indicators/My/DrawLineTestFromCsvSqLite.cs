#region Comments
/*
2022 012 14 1700  
	Code works but needs to be cleaned  
	Repeateted attmpts to use 'RemoveDrawObjects();'
	Only worked in script that draws lines  
	Need to add 'AllowRemovalOfDrawObjects = true;' in 'protected override void OnStateChange()'

2022 12 15 1440  
	

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
using NinjaTrader.Custom.AddOns;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
//using Microsoft.EntityFrameworkCore;
//using StringLibrary;
using LINQtoCSV;
//using NinjaTrader.Custom.AddOns;
#endregion

#region Status
/*
	2022 04 21 0500
		error: 'State.Configure: onvertedEndTime True
			Indicator '_Draw Line Test': Error on calling 'OnStateChange' method: Index (zero based) must be greater than or equal to zero and less than the size of the argument list.'

2022 12 12 2105  
	Uses NinjaTrader.Custom.Addons.NTDrawLine.cs for class (datacontext) to fill  
		from C:\data\csvNTDrawline.csv - 'returnedClass' Line  
	returnedClass has last Long_Short column blank and last row is duplicate of prior row  
	Error is in creating csvNTDrawline.csv  
*/
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.My
{
	public class DrawLineTestFromCsvSqLite : Indicator
	{
		protected override void OnStateChange()
		{
			AllowRemovalOfDrawObjects = true;
			if (State == State.SetDefaults)
			{
				Description = @"Enter the description for your new custom Indicator here.";
				Name = "DrawLineTest From Csv SqLite";
				Calculate = Calculate.OnBarClose;
				IsOverlay = true;
				DisplayInDataBox = true;
				DrawOnPricePanel = true;
				DrawHorizontalGridLines = true;
				DrawVerticalGridLines = true;
				PaintPriceMarkers = true;
				ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive = true;
				
			}
			else if (State == State.Configure)
			{

				//Print(String.Format("State.Configure: onvertedEndTime {0}", IsOverlay));
			}
			
			if (State == State.DataLoaded)
			{


                #region Call  CreateExtensionGetInsList()
				/*
                //	Not used when script is called from 'CallCreateInstListInd.cs'
                var input = new Paramaters.Input()
                {
                    BPlayback = false,
                    Name = "nq",
                    StartDate = "01/02/2023",
                    EndDate = "01/05/2023"
                };
*/
                //CreateExtensionGetInstList.Program.main(input);
                #endregion Call CallCreateExtensionGetInsList()


                #region Use LINQtoCSV to read "csvNTDrawline.csv"
                CsvFileDescription scvDescript = new CsvFileDescription();
				CsvContext cc = new CsvContext();				
				CsvFileDescription dataFromFile = new CsvFileDescription();

				//	Read in file 'C:\data\csvNTDrawline.csv'  Fills returnedClass
				//	Conflict with Class NTDrawLine - in both getInstListFutures and getInstListFuturesExtension 
				IEnumerable<NinjaTrader.Custom.AddOns.NTDrawLine> returnedClass = 
					cc.Read < NinjaTrader.Custom.AddOns.NTDrawLine >
                (
					@"C:\data\csvNTDrawline.csv",
					dataFromFile
				);
                #endregion Use LINQtoCSV to read "csvNTDrawline.csv"

                #region Define Brushes
                int i = 0;									//	Used for tag in Write.Line()
				string color = "";
				SolidColorBrush red = Brushes.Red;
				SolidColorBrush blue = Brushes.Blue;
				SolidColorBrush brush = Brushes.Blue;
				#endregion Define Brushes

				#region foreach() Through returnedClass and Draw line
				foreach (var rc in returnedClass)
				{
                    #region Print rc - Commented Out
                    /*
                    Print(String.Format("\ncombinedQry.csv: {0} {1} {2} {3} {4} {5} {6} {7} {8} ", 
						rc.Id, 
						rc.StartTimeTicks, 
						rc.StartTime, 
						rc.StartY, 
						rc.EndTimeTicks, 
						rc.EndTime, 
						rc.EndY, 
						rc.P_L, 
						rc.Long_Short));
					*/
				
                    #endregion Print rc - Commented Out

                    #region Determine brush color
                    // Long Win
                    if (rc.P_L >= 0 && rc.Long_Short == "Long")
					{
						brush = Brushes.Blue;
					}
					// Short Win
					else if(rc.P_L >= 0 && rc.Long_Short == "Short")
					{
						brush = Brushes.Blue;
					}
					// Long Loss
					else if(rc.P_L < 0 && rc.Long_Short == "Long") 
					{
						brush = Brushes.Red;
					}
					// Short Loss
					else if(rc.P_L < 0 && rc.Long_Short == "Short")
					{
						brush = Brushes.Red;
					};
					#endregion Determine brush color

					#region Draw.Line()
					Draw.Line
						(this, 
						//	i is line tag
						i.ToString(), 
						false, 
						DateTime.Parse(rc.StartTime)  , 
						rc.StartY, 
						DateTime.Parse( rc.EndTime) , 
						rc.EndY, 
						brush, 
						DashStyleHelper.Solid, 
						5);
                    #endregion Draw.Line()

                    i++;
				}
                #endregion foreach() Through returnedClass and Draw line

            }
        }
		protected override void OnBarUpdate()
		{

			//RemoveDrawObjects();
			if (CurrentBar < 20) return;


		}
	}
	
}

#region class NTDrawLine - Moved to AddOns
/*
public class NTDrawLine
{
	//DateTime startTime, double startY, DateTime endTime, double endY
	public int Id { get; set; }
	public string Long_Short { get; set; }
	public long StartTimeTicks { get; set; }
	public string StartTime { get; set; }
	public double StartY { get; set; }
	public long EndTimeTicks { get; set; }
	public string EndTime { get; set; }
	public double EndY { get; set; }
	public double P_L { get; set; }

	public NTDrawLine (){}
	
	public NTDrawLine (int id, string long_Short, long startTimeTicks, string startTime, double startY, long endTimeTicks, string endTime, double endY)
	{
		Id = id;
		Long_Short = long_Short;
		StartTimeTicks = startTimeTicks;
		StartTime = startTime;
		StartY = startY;
		EndTimeTicks = endTimeTicks;
		EndTime = endTime;
		EndY = endY;
	}
}
#endregion


#region Combined list from file
public class ReturnedClass
{
	public int EnId { get; set; }
	public int ExId { get; set; }
	public int EnAcc { get; set; }
	public long EnTime { get; set; }
	public string EnHuman { get; set; }
	public double EnPrice { get; set; }
	public long ExTime { get; set; }
	public string ExHuman { get; set; }
	public double ExPrice { get; set; }


	public ReturnedClass(){}
	
	public ReturnedClass(int enId, int exId, int enAcc, long enTime, string enHuman, double enPrice,
		long exTime, string exHuman, double exPrice)
	{
		 EnId = enId;
		 ExId = exId;
		 EnTime = enTime;
		 EnAcc = enAcc;
		 EnTime = enTime;
		 EnHuman = enHuman;
		 EnPrice = enPrice;
		 ExTime = exTime;
		 ExHuman = exHuman;
		 ExPrice = exPrice;
		}
}
*/
#endregion - Moved to AddOns

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.DrawLineTestFromCsvSqLite[] cacheDrawLineTestFromCsvSqLite;
		public My.DrawLineTestFromCsvSqLite DrawLineTestFromCsvSqLite()
		{
			return DrawLineTestFromCsvSqLite(Input);
		}

		public My.DrawLineTestFromCsvSqLite DrawLineTestFromCsvSqLite(ISeries<double> input)
		{
			if (cacheDrawLineTestFromCsvSqLite != null)
				for (int idx = 0; idx < cacheDrawLineTestFromCsvSqLite.Length; idx++)
					if (cacheDrawLineTestFromCsvSqLite[idx] != null &&  cacheDrawLineTestFromCsvSqLite[idx].EqualsInput(input))
						return cacheDrawLineTestFromCsvSqLite[idx];
			return CacheIndicator<My.DrawLineTestFromCsvSqLite>(new My.DrawLineTestFromCsvSqLite(), input, ref cacheDrawLineTestFromCsvSqLite);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.DrawLineTestFromCsvSqLite DrawLineTestFromCsvSqLite()
		{
			return indicator.DrawLineTestFromCsvSqLite(Input);
		}

		public Indicators.My.DrawLineTestFromCsvSqLite DrawLineTestFromCsvSqLite(ISeries<double> input )
		{
			return indicator.DrawLineTestFromCsvSqLite(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.DrawLineTestFromCsvSqLite DrawLineTestFromCsvSqLite()
		{
			return indicator.DrawLineTestFromCsvSqLite(Input);
		}

		public Indicators.My.DrawLineTestFromCsvSqLite DrawLineTestFromCsvSqLite(ISeries<double> input )
		{
			return indicator.DrawLineTestFromCsvSqLite(input);
		}
	}
}

#endregion
