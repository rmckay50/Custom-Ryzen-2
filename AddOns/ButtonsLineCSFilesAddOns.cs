#region Using declarations
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using LINQtoCSV;
using System.Data.SQLite;
#endregion

//This namespace holds Add ons in this folder and is required. Do not change it. 
namespace NinjaTrader.Custom.AddOns
{
    //public class ButtonsLineCSFilesAddOns : NinjaTrader.NinjaScript.AddOnBase
    //{
    public class CSV
    {
        //public int EntryId { get; set; }
        public int EntryId { get; set; }                                                                //	class CSV
        public string Name { get; set; }
        public int FilledBy { get; set; }                                                               //	class CSV
        public long? Qty { get; set; }                                                                    //	class CSV
        public long? RemainingExits { get; set; }                                                         //	class CSV
        public long? StartTimeTicks { get; set; }
        public string StartTime { get; set; }
        public double? Entry { get; set; }                                                               //	class CSV
        public long? EndTimeTicks { get; set; }
        public string EndTime { get; set; }
        public double? Exit { get; set; }                                                                //	class CSV
        public string Long_Short { get; set; }
        public double P_L { get; set; }
        public double? DailyTotal { get; set; }
        public int? TotalTrades { get; set; }


        public IEnumerator GetEnumerator()                                                              //	class CSV
        {
            return (IEnumerator)this;                                                                       //	class CSV
        }
    }
    public class Executions
    {
        public Int64 Id { get; set; }

        public Int64 Account { get; set; }

        public Int64? BarIndex { get; set; }

        public Double? Commission { get; set; }
        //public float? Commission { get; set; }

        public Int64? Exchange { get; set; }

        public String ExecutionId { get; set; }

        public Double? Fee { get; set; }

        public Int64 Instrument { get; set; }

        public Int64? IsEntry { get; set; }

        public bool? IsEntryB { get; set; }

        public Int64? IsEntryStrategy { get; set; }

        public Int64? IsExit { get; set; }

        public Boolean? IsExitB { get; set; }

        public Int64? IsExitStrategy { get; set; }

        public Int64? LotSize { get; set; }

        public Int64? MarketPosition { get; set; }

        public Double? MaxPrice { get; set; }

        public Double? MinPrice { get; set; }

        public String Name { get; set; }

        public String OrderId { get; set; }

        public Int64? Position { get; set; }

        public Int64? PositionStrategy { get; set; }

        public Double? Price { get; set; }

        public Int64? Quantity { get; set; }

        public Double? Rate { get; set; }

        public Int64? StatementDate { get; set; }

        public Int64? Time { get; set; }

        public Object ServerName { get; set; }
        public Executions() { }
        public Executions(long id, long account, long? barIndex, double? commission, long? exchange, string executionId, double? fee, long instrument, long? isEntry, long? isEntryStrategy, long? isExit, long? isExitStrategy, long? lotSize, long? marketPosition, double? maxPrice, double? minPrice, string name, string orderId, long? position, long? positionStrategy, double? price, long? quantity, double? rate, long? statementDate, long? time, object serverName)
        {
            Id = id;
            Account = account;
            BarIndex = barIndex;
            Commission = commission;
            Exchange = exchange;
            ExecutionId = executionId;
            Fee = fee;
            Instrument = instrument;
            IsEntry = isEntry;
            IsEntryStrategy = isEntryStrategy;
            IsExit = isExit;
            IsExitStrategy = isExitStrategy;
            LotSize = lotSize;
            MarketPosition = marketPosition;
            MaxPrice = maxPrice;
            MinPrice = minPrice;
            Name = name;
            OrderId = orderId;
            Position = position;
            PositionStrategy = positionStrategy;
            Price = price;
            Quantity = quantity;
            Rate = rate;
            StatementDate = statementDate;
            Time = time;
            ServerName = serverName;
        }
    }	
    public class Input
    {
        public bool BPlayback { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string TimeFirstBarOnChart { get; set; }
        public string TimeLastBarOnChart { get; set; }
		public long Expiry { get; set; }


        public Input()
        {
        }

        public Input(bool bPlayback, string name, string startDate, string endDate, string inputPath, string outputPath, string timeFirstBarOnChart, string timeLastBarOnChart, long expiry)
        {
            BPlayback = bPlayback;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            InputPath = inputPath;
            OutputPath = outputPath;
            TimeFirstBarOnChart = timeFirstBarOnChart;
            TimeLastBarOnChart = timeLastBarOnChart;
            Expiry = expiry;
        }

    }
    public static class Methods
    {
        //public static List<Ret> instList = new List<Ret>();
        public static List<Ret> getInstList(string name,
            string startDate, string endDate, bool bPlayback, string pathFromCall)
        {
            //var path = @"Data Source = C:\Users\Owner\Documents\NinjaTrader 8\db\NinjaTrader.sqlite";
            var path = pathFromCall;
            //  list to hold valiables in Executions table from NinjaTrader.sqlite
            List<Executions> listExecution = new List<Executions>();
            //  list to hold Ret() format from listExecution
            List<Ret> listExecutionRet = new List<Ret>();
            //List<Query> selectedList = new List<Query>();
            //List<Ret> instList = new List<Ret>();
            var instList = new List<Ret>();

            //List<Query> listFromQuery = new List<Query>();

            //  Below is code from getInstList for filling in Expiry
            //  Expiry is located in Instruments 
            //  Can either load Instruments and do query or get info from chart 
            //  Will try char and in interim just add string 'Dec 2022'
            //  Format from .sdf instList was 'Dec 2022' 
            //  Expiry = new DateTime((long)mInsIns.Expiry).ToString(" MMM yyyy"),
            //  public string Expiry { get; set; }
            //var symbol = "NQ";
            //var instrument = 699839150758595;



            // Convert dates to Utc ticks
            // Used for start and stop times
            DateTime sDate = DateTime.Parse(startDate);                         //.Dump("sDate")
            DateTime sDateUtc = TimeZoneInfo.ConvertTimeToUtc(sDate);           //.Dump("sDateUtc")
            var startTicks = sDateUtc.Ticks;
            DateTime eDate = DateTime.Parse(endDate);                           //.Dump("eDate")
            DateTime eDateUtc = TimeZoneInfo.ConvertTimeToUtc(eDate);           //.Dump("eDateUtc")
            var endTicks = eDateUtc.Ticks;

            using (var db = new System.Data.SQLite.SQLiteConnection(path))
            {
                try
                {
                    db.Open();
                    SQLiteDataReader reader;
                    SQLiteCommand sqlite_cmd;
                    sqlite_cmd = db.CreateCommand();
                    sqlite_cmd.CommandText = "SELECT * FROM Executions";

                    reader = sqlite_cmd.ExecuteReader();

                    //  used to hold data after read from db
                    while (reader.Read())
                    {
                        Executions exec = new Executions();
                        exec.Id = (long)reader[0];
                        exec.Account = (long)reader[1];
                        exec.BarIndex = (long)reader[2];
                        exec.Commission = (System.Double)reader[3];
                        exec.Exchange = (long)reader[4];
                        exec.ExecutionId = (string)reader[5];
                        exec.Fee = (double)reader[6];
                        exec.Instrument = (long)reader[7];
                        exec.IsEntry = (long)reader[8];
                        exec.IsEntryStrategy = (long)reader[9];
                        exec.IsExit = (long)reader[10];
                        exec.IsExitStrategy = (long)reader[11];
                        exec.LotSize = (long)reader[12];
                        exec.MarketPosition = (long)reader[13];
                        exec.MaxPrice = (System.Double)reader[14];
                        exec.MinPrice = (System.Double)reader[15];
                        exec.Name = (string)reader[16];
                        exec.OrderId = (string)reader[17];
                        exec.Position = (long)reader[18];
                        exec.PositionStrategy = (long)reader[19];
                        exec.Price = (System.Double)reader[20];
                        exec.Quantity = (long)reader[21];
                        exec.Rate = (double)reader[22];
                        exec.StatementDate = (long)reader[23];
                        exec.Time = (long)reader[24];
                        exec.ServerName = (string)reader[25];

                        //  add row to list
                        listExecution.Add(exec);
                    }
                    //  close reader
                    reader.Close();
                    db.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                //  change IsEntry and IsExit to bool?
                foreach (var entry in listExecution)
                {
                    //  IsEntry to bool?
                    if (entry.IsEntry == 0)
                    {
                        entry.IsEntryB = false;
                    }
                    else
                    {
                        entry.IsEntryB = true;
                    }

                    //  IsExit to bool?
                    if (entry.IsExit == 0)
                    {
                        entry.IsExitB = false;
                    }
                    else
                    {
                        entry.IsExitB = true;
                    }
                }

            }
            try
            {
                foreach (var execList in listExecution)
                {
                    //	create ListExecutionQueryClass
                    Ret list = new Ret();
                    {
                        //	fill new list 
                        list.InstId = (long?)0;
                        list.ExecId = execList.Id;
                        list.Name = name;
                        list.Account = execList.Account;
                        list.Position = execList.Position;
                        list.Quantity = execList.Quantity;
                        list.IsEntry = execList.IsEntryB;
                        list.IsExit = execList.IsExitB;
                        list.Price = execList.Price;
                        list.Time = execList.Time;
                        list.HumanTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime((long)execList.Time), TimeZoneInfo.Local).ToString("  HH:mm:ss MM/dd/yyyy  ");
                        list.Instrument = execList.Instrument;
                        list.P_L = 0;
                        list.Long_Short = "";

                        //	add to list
                        listExecutionRet.Add(list);
                    }
                }
                //  list from Executions table in Ret() format
                //  query this list with 'Instrument'
                //listExecutionRet.ToList();

            }
            catch
            {
                Console.WriteLine("foreach (var execList in listExecution)");
            }

            ///<summary>
            ///<param>query list from Executions in Ret() - listExecutionRets</param>
            ///<param>did not fill in expiry - found in Instruments with instrument #</param>
            /// </summary>
            try
            {
                //  If bPlayback == true set account == 1
                //  default is account == 2
                var account = 2;
                if (bPlayback == true)
                {
                    account = 1;
                }
                instList = (from list in listExecutionRet
                            where list.Time > sDateUtc.Ticks && list.Time < eDateUtc.Ticks
                            where list.Account == account
                            select new Ret()
                            {
                                InstId = (long?)0,
                                ExecId = list.ExecId,
                                Account = list.Account,
                                Name = list.Name,
                                Position = list.Position,
                                Quantity = list.Quantity,
                                IsEntry = list.IsEntry,
                                IsExit = list.IsExit,
                                Price = list.Price,
                                Time = list.Time,
                                HumanTime = list.HumanTime,
                                Instrument = list.Instrument,
                                Expiry = "Dec 2022",
                                P_L = 0,
                                Long_Short = ""

                            }).ToList();
                instList = instList.OrderByDescending(e => e.ExecId).ToList();

                // add Id to selectetRetList
                var instId = 0;
                foreach (Ret r in instList)
                {
                    r.InstId = instId;
                    instId++;
                }


            }
            catch
            {
                Console.WriteLine("query list from Executions");
            }

            return (List<Ret>)instList.ToList();

        }
    }
    public class NTDrawLine
    {
        //DateTime startTime, double startY, DateTime endTime, double endY
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Long_Short { get; set; }
        public long StartTimeTicks { get; set; }
        public string StartTime { get; set; }
        public double StartY { get; set; }
        public long EndTimeTicks { get; set; }
        public string EndTime { get; set; }
        public double EndY { get; set; }
        public double P_L { get; set; }
        public double? DailyTotal { get; set; }
        public int? TotalTrades { get; set; }

        public NTDrawLine() { }

        public NTDrawLine(int id, string symbol, string long_Short, long startTimeTicks, string startTime, double startY, long endTimeTicks, string endTime, double endY,
            double p_L, double? dailyTotal, int? totalTrades)
        {
            Id = id;
            Symbol = symbol;
            Long_Short = long_Short;
            StartTimeTicks = startTimeTicks;
            StartTime = startTime;
            StartY = startY;
            EndTimeTicks = endTimeTicks;
            EndTime = endTime;
            EndY = endY;
            P_L = p_L;
            DailyTotal = dailyTotal;
            TotalTrades = totalTrades;
        }
    }
    public class NTDrawLineForLINQtoCSV
    {
        [CsvColumn(FieldIndex = 1)]
        public int Id { get; set; }
        [CsvColumn(FieldIndex = 2)]
        public string Symbol { get; set; }
        [CsvColumn(FieldIndex = 3)]
        public string Long_Short { get; set; }
        [CsvColumn(FieldIndex = 4)]
        public long StartTimeTicks { get; set; }
        [CsvColumn(FieldIndex = 5)]
        public string StartTime { get; set; }
        [CsvColumn(FieldIndex = 6)]
        public double StartY { get; set; }
        [CsvColumn(FieldIndex = 7)]
        public long EndTimeTicks { get; set; }
        [CsvColumn(FieldIndex = 8)]
        public string EndTime { get; set; }
        [CsvColumn(FieldIndex = 9)]
        public double EndY { get; set; }
        [CsvColumn(FieldIndex = 10)]
        public double P_L { get; set; }
        [CsvColumn(FieldIndex = 11)]
        public double? DailyTotal { get; set; }
        [CsvColumn(FieldIndex = 12)]
        public int? TotalTrades { get; set; }

    }
    public class Source
    {
        public int ActiveEntryId { get; set; }                                                          //	class source
        public long? ActiveEntryRemaining { get; set; }                                                   //	class source
        public double? ActiveEntryPrice { get; set; }                                                    //	class source
        public bool IsReversal { get; set; }                                                            //	class source
        public long PositionAfterReverse { get; set; }                                                   //	class source
        public long RowOfReverse { get; set; }                                                           //	class source
        public long Position { get; set; }                                                               //	class source
        public double? StartingExitPrice { get; set; }                                                   //	class source
        public int rowInTrades { get; set; }                                                            //	class source
        public int RowInTrades { get; set; }                                                            //	class source
        //public int RowInCsv { get; set; }                                                               //	class source
        public long? ExitQty { get; set; }                                                                //	class source
        public long? Remaining { get; set; }                                                              //	class source
        public List<Trade> Trades { get; set; }                                                         //	class source
        public List<CSV> Csv { get; set; }                                                              //	class source
        public List<NTDrawLine> NTDrawLine { get; set; }

    }
    public class Ret
    {
        //  InstId is an Id for this class and is filled in later
        public long? InstId { get; set; }         // Created when instList is made
        public long ExecId { get; set; }
        public long Account { get; set; }
        public string Name { get; set; }
        public long? Position { get; set; }
        public long? Quantity { get; set; }
        public bool? IsEntry { get; set; }
        public bool? IsExit { get; set; }
        public double? Price { get; set; }
        public long? Time { get; set; }
        public string HumanTime { get; set; }
        public long Instrument { get; set; }
        //Expiry is located innInstruent list and will no be used
        public string Expiry { get; set; }
        public double? P_L { get; set; }
        public string Long_Short { get; set; }

        public Ret() { }

        public Ret(long instId, long execId, long account, string name, long? position, long? quantity, bool? isEntryL, bool? isExit, double? price, long? time, string humanTime, long instrument, string expiry, double? p_L, string long_Short)
        {
            InstId = instId;
            ExecId = execId;
            Name = name;
            Account = account;
            Position = position;
            Quantity = quantity;
            IsEntry = isEntryL;
            IsExit = isExit;
            Price = price;
            Time = time;
            HumanTime = humanTime;
            Instrument = instrument;
            Expiry = expiry;
            P_L = p_L;
            Long_Short = long_Short;
        }
    }
    public class Trade
    {
        public int Id { get; set; }
        public long ExecId { get; set; }                                                                     // 	class Trade
        public string Name { get; set; }
        public long? Position { get; set; }                                                                    // 	class Trade
        public long? Qty { get; set; }                                                                    // 	class Trade
        public bool? IsEntry { get; set; }                                                                 // 	class Trade
        public bool? IsExit { get; set; }                                                                  // 	class Trade
        public bool IsRev { get; set; }                                                                 // 	class Trade
        public bool Matched { get; set; }                                                               // 	class Trade
        public double? Price { get; set; }                                                               // 	class Trade
        public long? Time { get; set; }
        public string HumanTime { get; set; }
        public long Instrument { get; set; }
        public string Expiry { get; set; }
        public double? P_L { get; set; }
        public string Long_Short { get; set; }
        public int TradeNo { get; set; }                                                                // 	class Trade

        public IEnumerator GetEnumerator()                                                              // 	class Trade
        {
            return (IEnumerator)this;                                                                   // 	class Trade
        }

        public Trade() { }

        // Without int execId (second cstr, code runs
        //public Ret(int instId, int execId, string name, int? position, int? quantity, bool? isEntry, bool? isExit, double? price, long? time,
        //	string humanTime, long instrument, string expiry, double? p_L, string long_Short)
        public Trade(long execId, long? position, string name, long? qty, bool? isEntry, bool? isExit, double? price, long? time,
            string humanTime, long instrument, string expiry, double? p_L, string long_Short)
        {
            //InstId = instId;
            ExecId = execId;
            Position = position;
            Name = name;
            Qty = qty;
            IsEntry = isEntry;
            IsExit = isExit;
            Price = price;
            Time = time;
            HumanTime = humanTime;
            Instrument = instrument;
            Expiry = expiry;
            P_L = p_L;
            Long_Short = long_Short;
        }

        public Trade(int id)
        {
            Id = id;
        }
    }
}
