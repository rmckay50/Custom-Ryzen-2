using System;
using System.Collections.Generic;
using System.Linq;
//using Executions = ExecutionsClass.Executions;
//using Query = ListExecutionQueryClass.ListExecutionQuery;
//using ExtensionCreateNTDrawline;
using LINQtoCSV;
using System.IO;

namespace NinjaTrader.Custom.AddOns
{
    public class SqLiteExecutionsToListAndQueryResults
    {
        public class Program
        {
            #region Set Parameters
            ////	Set to true for playback (account - 1)
            //  These parameters are passed from calling progran
            public static bool bPlayback = false;
            public static string name = "nq";
            public static string startDate = "  10:40:56 03/22/2023  ";
            // Set to "12/12/2022" to get all of data
            public static string endDate = "03/24/2022";
            #endregion Set Parameters



            /// <summary>
            /// <switch between static void Main(string[] args) & public static void main(Paramaters.Input input)
            ///     to use for .exe or .dll
            ///     Parameters.Input input is filled in in callin program 
            /// <param name="input"></param>
            /// </summary>
            /// 
            //static void Main(string[] args)
            //  can use 'Input' becaue using statement is 'using Parameters.Paramaters;'
            //public static void main(Parameters.Input input, string output)
//            public static void main(Parameters.Input input, List<Trade> workingTrades)
				public static void main(Input input)


            {
                var timeIn = input.TimeLastBarOnChart;
                var path = input.InputPath;

                List<CSV> CSv = new List<CSV>();
                //  list to hold valiables in Executions table from NinjaTrader.sqlite
                List<Executions> listExecution = new List<Executions>();
                List<NTDrawLine> nTDrawline = new List<NTDrawLine>();
                //  list to hold Ret() format from listExecution
                List<Ret> listExecutionRet = new List<Ret>();
                Source source = new Source();
                List<Trade> workingTrades = new List<Trade>();
                List<Trade> trades = new List<Trade>();


                ///
                ///<summary>
                /// <as.exe> use the line 'Input input = new Input()'</as.exe>
                /// <as.dll> use the section  ''</as.dll>
                /// </summary>
                /// 
                #region Uncomment for use as .exe
                //Input exeInput = new Input()
                //{
                //    BPlayback = false,
                //    Name = "nq",
                //    StartDate = "01/02/2022",
                //    EndDate = "02/05/2023"
                //};
                #endregion Uncomment for use as .exe

                var instList = Methods.getInstList
                    (
                #region Uncomment for use as .exe
                    //name,
                    //startDate,
                    //endDate,
                    //bPlayback,
                    //@"
                #endregion Uncomment for use as .exe

                #region Uncomment for use as .dll
                    input.Name,
                    input.StartDate,
                    input.EndDate,
                    input.BPlayback,
                    input.InputPath
                #endregion Uncomment for use as .dll
                    );

                #region Create workingTrades

                //  NT runs through this section more than once
                //      Allow only one pass
                if (trades.Count == 0)
                {
                    //	Create 'workingTrades' list																		//	Main
                    //	Slimmed down instList that is added to source list to make transfer to extension easier
                    // 	foreach through instList and add to trades list

                    foreach (var inst in instList)
                    {
                        trades.Add(new Trade(inst.ExecId, inst.Position, inst.Name, inst.Quantity, inst.IsEntry, inst.IsExit, inst.Price,
                            inst.Time, inst.HumanTime, inst.Instrument, inst.Expiry, inst.P_L, inst.Long_Short));
                    }
                }
                //	Top row in Trades is last trade.  Position should be zero.  If not db error or trade was exited 
                //		next day
                //	Check that position is flat
                //if (t.Id == 0 && t.IsExit == true)
                //  Need to change from Console.WriteLine() to NT Print
                try
                {
                    if (trades[0].Position != 0)

                    {
                        Console.WriteLine(@"Postion on - not flat");                                                  //	Main
                        Console.WriteLine(string.Format("Trades position = {0}", (trades[0].Position)));        //System.Environment.Exit(-1);                                                                //	Main																
                    }
                }
                catch
                {
                }
                //	Top row is now first trade in selected list - Position != 0
                trades.Reverse();
                workingTrades = trades.ToList();
                trades.Clear();
                #endregion Create workingTrades				

                #region Code from 'Fill finList Prices Return List and Csv from Extension'							//	Main

                #region Create Lists
                //  Lists added to source which is used in extensions
                source.Trades = workingTrades;
                source.Csv = CSv;                                                                                   //	Main
                source.NTDrawLine = nTDrawline;
                #endregion Create Lists													

                #region Initialize flags and variables in source
                //	'rowInTrades' is increased on each pass of foreach(var t in workingTrades)
                //	It is number of the row in trades that is either an Entry, Exit, or Reversal
                source.rowInTrades = 0;

                //	isReversal is flag for reversal
                source.IsReversal = false;                                                                          //	Main
                #endregion Initialize flags and variables in source

                #region Fill in Id	Not used on Ryzen-2																				//	Main
                //	Add Id to workingTrades
                int i = 0;                                                                                          //	Main
                //  var workingTrades2 = workingTrades.ToList();

                foreach (var t in workingTrades)                                                                        //	Main
                {
                    t.Id = i;                                                                                       //	Main
                    i++;                                                                                            //	Main
                }
                #endregion Fill in Id																					//	Main

                #region foreach() through source.Trades
                foreach (var t in source.Trades)                                                                    //	Main
                {
                    //	Record size of first entry and Id
                    //	Need to keep record of how many entries are matched on split exits
                    //	Updated in UpdateActiveEntery()
                    if (t.Id == 0 && t.IsEntry == true)
                    {
                        source.ActiveEntryId = t.Id;                                                                //	Main
                        source.ActiveEntryRemaining = t.Qty;                                                        //	Main
                        source.ActiveEntryPrice = t.Price;                                                          //	Main
                    }

                    //	Is trade a normal exit?
                    //	If previous trade was reversal the source.Trades.IsRev is == true
                    //if (t.Entry == false && t.Exit == true && source.Trades[source.rowInTrades - 1].IsRev == false) //	Main
                    if (t.IsEntry == false && t.IsExit == true) //	Main
                    {
                        source.Fill();
                    }


                    //	Set reversal flags row numbers
                    if (t.IsEntry == true && t.IsExit == true)                                                          //	Main
                    {
                        //	Set source.IsReversal = true - used to break out of Fill()
                        source.IsReversal = true;                                                                   //	Main
                        source.RowOfReverse = source.rowInTrades;                                                   //	Main
                        source.RowInTrades = source.rowInTrades;                                                    //	Main
                        source.rowInTrades = source.rowInTrades;                                                    //	Main
                        source.Fill();                                                                              //	Main		
                    }

                    source.rowInTrades++;  // = rowInTrades;														//	Main
                                           //	Increase source.rowInTrades it was cycled through in the Fill extension
                    source.RowInTrades++;                                                                           //	Main
                }

                #endregion foreach() through source.Trades

                #endregion Code from 'Fill finList Prices Return List and Csv from Extension'										

                #region FillLongShortColumnInTradesList		

                //	Call extenstion 'FillLongShortColumnInTradesList()' to fill in Long_Short column in workingTrades 
                source.FillLongShortColumnInTradesList();

                #endregion FillLongShortColumnInTradesList

                #region foreach through .csv and add StartTimeTicks StartTime ExitTimeTicks ExitTime Long_Short

                foreach (var csv in source.Csv)
                {
                    //	fill in blank spaces from workingTrades with time and tick//

                    csv.Name = workingTrades[csv.EntryId].Name;
                    csv.StartTimeTicks = workingTrades[csv.EntryId].Time;
                    csv.StartTime = workingTrades[csv.EntryId].HumanTime;
                    csv.EndTimeTicks = workingTrades[csv.FilledBy].Time;
                    csv.EndTime = workingTrades[csv.FilledBy].HumanTime;
                    csv.Long_Short = workingTrades[csv.EntryId].Long_Short;
                }

                #endregion foreach through .csv and add StarTimeTicks StartTime ExitTimeTicks ExitTime

                #region Fill in P_L coulmn in source.csv
                //	Call 'FillProfitLossColumnInTradesList' to fill in csv P_L column
                source.FillProfitLossColumnInTradesList();
                //source.
                #endregion Fill in P_L coulmn in source.csv

                #region Create NTDrawLine list for use in saving to file and later in NT

                source.NTDrawLine = source.CreateNTDrawline();

                #endregion Create NTDrawLine list for use in saving to file and later in NT

                #region Use LINQtoCSV on combined list to write
                //  foreach through source.NTDrawLine to create list with correct order for cc.write
                //  uses 'NTDrawLineForLINQtoCSV' which has column attributes
                var columnsWithAttributes = from l in source.NTDrawLine
                                            select new NTDrawLineForLINQtoCSV
                                            {
                                                Id = l.Id,
                                                Symbol = l.Symbol,
                                                Long_Short = l.Long_Short,
                                                StartTimeTicks = l.StartTimeTicks,
                                                StartTime = l.StartTime,
                                                StartY = l.StartY,
                                                EndTimeTicks = l.EndTimeTicks,
                                                EndTime = l.EndTime,
                                                EndY = l.EndY,
                                                P_L = l.P_L,
                                            };
                columnsWithAttributes.ToList();

                CsvFileDescription scvDescript = new CsvFileDescription();
                CsvContext cc = new CsvContext();
                cc.Write
                (
                source.NTDrawLine,
                input.OutputPath
                );

                //  replace name (local declaration) to input.Name (calling program definition)
                var fileName = input.Name.ToUpper() + "                " + input.TimeFirstBarOnChart + ".csv";
                var dir = Path.GetDirectoryName(input.OutputPath); ;

                if (input.BPlayback != true)
                {
                    cc.Write(source.NTDrawLine, dir + @"\" + fileName);
                }
                else
                {
                    //  lastBarTime is set in NT 'ChartBars.GetTimeByBarIdx(ChartControl, ChartBars.ToIndex)); //8/11/2015 4:30:00 AM'
                    string l = input.TimeLastBarOnChart;
                    fileName = input.Name.ToUpper() + " Playback " + input.TimeFirstBarOnChart + " To " + input.TimeLastBarOnChart + ".csv";
                    cc.Write(source.NTDrawLine, dir + @"\" + fileName);
                }

                #endregion


            }
        }

    }


}
