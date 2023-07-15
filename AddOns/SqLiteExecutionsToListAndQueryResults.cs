using System;
using System.Collections.Generic;
using System.Linq;
using LINQtoCSV;
using System.IO;
using NinjaTrader.NinjaScript;
using NinjaTrader.Custom.AddOns.Properties;
using System.Collections;

namespace NinjaTrader.Custom.AddOns
{
    public class SqLiteExecutionsToListAndQueryResults
    {
        public class Program
        {

            /// <summary>
            /// <switch between static void Main(string[] args) & public static void main(Paramaters.Input input)
            ///     to use for .exe or .dll
            ///     Parameters.Input input is filled in in callin program 
            /// <param name="input"></param>
            /// </summary>
            /// 
            //  can use 'Input' becaue using statement is 'using Parameters.Paramaters;'

            public static bool Main(Input parameters)
            {

                List<CSV> CSv = new List<CSV>();

                //  list to hold valiables in Executions table from NinjaTrader.sqlite
                List<Executions> listExecution = new List<Executions>();
                List<NTDrawLine> nTDrawline = new List<NTDrawLine>();
                //  list to hold Ret() format from listExecution
                List<Ret> listExecutionRet = new List<Ret>();
                Source source = new Source();
                List<Trade> workingTrades = new List<Trade>();
                List<Trade> trades = new List<Trade>();
                var isEmpty = false;
                IEnumerable<NTDrawLine> returnedClass;
                //  combined existing list and new list
                IEnumerable<NTDrawLine> listToPrint = new List<NTDrawLine>();
                List<NTDrawLine> listToPrintWithDailies = new List<NTDrawLine>();

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

                //var instList = Methods.getInstList
                //    (
                //#region Uncomment for use as .exe
                //    //name,
                //    //startDate,
                //    //endDate,
                //    //bPlayback,
                //    //@"
                //#endregion Uncomment for use as .exe

                //#region Uncomment for use as .dll
                //    input.Name,
                //    input.StartDate,
                //    input.EndDate,
                //    input.BPlayback,
                //    input.InputPath
                //#endregion Uncomment for use as .dll
                //    );
                /********************************************************************************/
                var instList = Methods.getInstList
                    (
                #region Uncomment for use as .exe
                    //  parameters                
                #endregion Uncomment for use as .exe

                #region Uncomment for use as .dll
                    parameters
                #endregion Uncomment for use as .dll
                    );

                //  check for empty list - means symbol not found
                if ( instList.Count == 0 )
                {
                    isEmpty = true;
                    return isEmpty;
                }

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

                #region Fill in P_L column in source.csv
                //	Call 'FillProfitLossColumnInTradesList' to fill in csv P_L column
                source.FillProfitLossColumnInTradesList();
                //source.
                #endregion Fill in P_L coulmn in source.csv

                #region Fill in Daily Total Column

                ////	Call 'FillDailyTotalColumn' to fill in csv Daily Total column
                //source.FillDailyTotalColumn();

                #endregion Fill in Daily Total Column


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
                                                DailyTotal = l.DailyTotal,
                                                TotalTrades = l.TotalTrades
                                            };
                columnsWithAttributes.ToList();

                //  bPlayback != true:
                //      create CsvFileDescription
                //      create and write to 'csvNTDrawline.csv'
                //
                //  bPlayback == true:
                //      appendPlayback == true && firstPassAppend == true
                //          create CsvFileDescription
                //          create and write to 'csvNTDrawline.csv'
                //          firstPassAppend = false
                //      
                //      appendPlayback == true && firstPassAppend == false ( second pass )
                //          create CsvFileDescription with write headers set to false
                //          append to 'csvNTDrawline.csv'
                //  
                //   'csvNTDrawline.csv' will start over each time script is reloaded
                //
                //  stored properties are:
                //        Name                      Default  
                //      storedDate                  01/01/2000
                //      

                //  set up date for persistence when reloading - the script variables are reset
                var storedDate = NinjaTrader.Custom.AddOns.Properties.Settings.Default.storedDate;
                var dateNow = DateTime.Now.ToString("MM/dd/yyyy");
                var firstPassAppend = Settings.Default.firstPassAppend;


                //  if they are the same this is the first pass
                //  append to csvNTDrawline
                if ( storedDate != dateNow )
                {
                    Settings.Default.storedDate = DateTime.Now.ToString("MM/dd/yyyy");
                    Properties.Settings.Default.Save();
                }
                //  write to csvNTDrawline if not in Playback mode
                if (parameters.BPlayback == false)
                {
                    CsvFileDescription scvDescript = new CsvFileDescription();
                    CsvContext cc = new CsvContext();
                    //  write to parameters.OutputPath - normally csvNTDrawline
                    cc.Write
                    (
                    columnsWithAttributes,
                    parameters.OutputPath
                    );
                }
                //  this section is used when bPlayback is true
                //  AppendPlay is initialized to true in 'State.SetDefaults'
                //  firstPassAppend = Settings.Default.firstPassAppend is initialized from .setings
                //  needs to be set to true for first pass which will add column titles to csvNTDrawline
                else
                {
                    //  create and write to 'csvNTDrawline.csv'
                    if (parameters.AppendPlayback == true && firstPassAppend == true )
                    {
                        CsvFileDescription scvDescript = new CsvFileDescription();
                        CsvContext cc = new CsvContext();
                        //  write to parameters.OutputPath - normally cscNTDrawline
                        cc.Write
                        (
                        columnsWithAttributes,
                        parameters.OutputPath
                        );
                        //  set firstPass flag to false
                        Settings.Default.firstPassAppend = false;
                        Properties.Settings.Default.Save();
                    }
                    else if (parameters.AppendPlayback == true && firstPassAppend == false)
                    {
                        //  read existing csvNTDrawline.csv
                        #region Use LINQtoCSV to read "csvNTDrawline.csv"

                        //CsvFileDescription scvDescript = new CsvFileDescription();
                        CsvContext cc = new CsvContext();

                        //  receives file data during Read
                        CsvFileDescription dataFromFile = new CsvFileDescription();

                        //	Read in file 'C:\data\csvNTDrawline.csv'  Fills returnedClass
                        returnedClass = cc.Read<NTDrawLine>
                                        (
                                            parameters.OutputPath,
                                            dataFromFile
                                        );
                        #endregion Use LINQtoCSV to read "csvNTDrawline.csv"

                        #region Compare trades in existing .csv file with columnsWithAttributes
                        //  convert columnsWithAttributes to NTDrawLine - type mismatch that I couldn't resolve
                        var columns = from l in columnsWithAttributes
                                          select new NTDrawLine
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
                                                            DailyTotal = l.DailyTotal,
                                                            TotalTrades = l.TotalTrades
                                                        };
                            columns.ToList();

                        //foreach (var line in returnedClass)
                        //{
                        //    foreach (var column in columns)
                        //    {
                        //        //  compare each line of existing file with new list by start time
                        //        //  when there is a difference (existing trade or new trade) add it to list
                        //        if (column.StartTime == line.StartTime)
                        //        {
                        //            //  add new line to listToPrint
                        //            listToPrint = (IEnumerable<NTDrawLine>)
                        //                (from l in columnsWithAttributes
                        //                 select new NTDrawLine
                        //                 {
                        //                     Id = l.Id,
                        //                     Symbol = l.Symbol,
                        //                     Long_Short = l.Long_Short,
                        //                     StartTimeTicks = l.StartTimeTicks,
                        //                     StartTime = l.StartTime,
                        //                     StartY = l.StartY,
                        //                     EndTimeTicks = l.EndTimeTicks,
                        //                     EndTime = l.EndTime,
                        //                     EndY = l.EndY,
                        //                     P_L = l.P_L,
                        //                     DailyTotal = l.DailyTotal,
                        //                     TotalTrades = l.TotalTrades
                        //                 });
                        //            listToPrint.ToList();
                        //        }
                        //    }
                        //}
                        //#endregion Compare trades in existing .csv file with columnsWithAttributes
                        //int fu = 2;
                        //#region Recalculate daily totals

                        //List<NTDrawLine> x = listToPrint.ToList();
                        // var xreturn = x.FillDailyTotalColumn();

                        //listToPrintWithDailies = listToPrint.ToList().FillDailyTotalColumn();
                        //listToPrintWithDailies = Methods.FillDailyTotalColumn((List<NinjaTrader.Custom.AddOns.NTDrawLine>)listToPrint);

                        #endregion Recalculate daily totals

                        //foreach (var n in listToPrint)
                        //    //  get date ("MM/dd/yyyy") portion of end date
                        //    //  compare on each pass with starting date
                        //    //  when date changes (string compare) enter new total into DailyTotal column
                        //    listToPrint.ElementAt(1);
                        //var startingDate = listToPrint.ElementAt(0).EndTime.Substring(11);


                        ////  use to get trade end date to be used for comparison
                        //var currentTradeDate = "";

                        ////  use as register to total trade P/L values
                        ////  initialize with first value because starting poing for foreach is line 2
                        //double runningTotal = source[0].P_L;

                        ////  use as register to count number of trades in the day
                        //int TotalTrades = 1;

                        ////  need to keep track of line number in list
                        //int iD = 0;

                        ////  cycle through trades - compare trade end date with previous - record total on change
                        ////   zero accumulator
                        //foreach (var c in source)
                        //{
                        //    //  get date of trade ("/MM/dd/yyy")
                        //    currentTradeDate = c.EndTime.Substring(11);

                        //    //  has date changed - value less than zero is change
                        //    if (currentTradeDate.CompareTo(startingDate) == 0 && iD != 0)
                        //    {
                        //        //  add curent line P/L to accumulator
                        //        runningTotal = runningTotal + c.P_L;

                        //        //  add to number of days trades
                        //        TotalTrades++;
                        //    }

                        //    //  date has changed
                        //    else if (iD != 0)
                        //    {
                        //        //  insert total in DailyTotal column 1 line up
                        //        source[iD - 1].DailyTotal = runningTotal;

                        //        //  insert total in TotalTrades column 1 line up
                        //        source[iD - 1].TotalTrades = TotalTrades;


                        //        //  zero accumulator - this if is hit when dates are unequal so running total 
                        //        //      needs to be set to rows P/L - zero is not needed
                        //        runningTotal = 0;

                        //        //  zero TotalTrades
                        //        TotalTrades = 1;

                        //        //  add curent line P/L to accumulator
                        //        runningTotal = runningTotal + c.P_L;

                        //        //  update trade end date
                        //        startingDate = currentTradeDate;
                        //    };

                        //    //  update line ID
                        //    iD++;

                        //    //  if ID  == list.count - at end of list - enter last total
                        //    if (iD == source.Count)
                        //    {
                        //        source[iD - 1].DailyTotal = runningTotal;

                        //        //  enter number of trades in TotalTrades
                        //        source[iD - 1].TotalTrades = TotalTrades;

                        //    }
                        //}




                        #region Convert list with attributes to get correct order

                        var printWithAttributes = from l in listToPrint
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
                                                        DailyTotal = l.DailyTotal,
                                                        TotalTrades = l.TotalTrades
                                                    };
                        printWithAttributes.ToList();

                        #endregion Convert list with attributes to get correct order


                        cc.Write
                        (
                            //columnsWithAttributes,
                            printWithAttributes,
                            parameters.OutputPath
                        );

                        //  create a new file description that will does not use file headers
                        //var csvDescAppend = new CsvFileDescription();
                        //csvDescAppend.FirstLineHasColumnNames = false;
                        //csvDescAppend.EnforceCsvColumnAttribute = true;
                        //CsvContext ccApend = new CsvContext();

                        //  write using StreamWriter to csvNTDrawline
                        //  fileName, true - true is append (now with no columns)
                        //using (var stream = new StreamWriter(parameters.OutputPath, true))

                        //{
                        //    //ccApend.Write(columnsWithAttributes, stream, csvDescAppend);
                        //    //ccApend.Write(listToPrint, stream, csvDescAppend);
                        //    //cc.Write(listToPrint, stream, csvDescAppend);

                        //    cc.Write
                        //    (
                        //    columnsWithAttributes,
                        //    parameters.OutputPath
                        //    );



                        //    //listToPrint
                        //}

                    }
                }



                #endregion Use LINQtoCSV on combined list to write
                isEmpty = false;
                return isEmpty;

            }
        }

    }
}

