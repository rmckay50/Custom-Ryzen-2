### NinjaTrader.Custom

2023 02 19 0430  
*	commit - copying 'RecordAndDisplayTradesWithButtons' to 'RecordAndDisplayTradesWithButtonsRyzen2' because of crashes during runtime  

2023 02 19 1440  
*	commit - deleted all but a couple of the trades on 2/17  is creating the .csv file but too long for time  

2023 03 03 2105  
*	commit - for some reason the 'ExtensionMatchAndAddToCsv.dll' is stuck in a loop and it runs out of memory  
	worked through 'ExtensionMatchAndAddToCsv.dll'  
	used 'TSLA' and dates 2023 03 21 - 22 
	doesn't hang  

2023 03 04 1115  
*	commit - writing csvNTDrawline.csv and reading correctly  
	TODO  
	get data from TradeStation to work  

2023 03 09 2005  
*	commit - removed 'ReadCsvAndDrawLines()' from State == State.Historical & State == State.Transition = still crashing  
	set up serilog to see how far indicator (RecordAndDisplayTradesWithButtonsRyzen2) gets  

2023 03 11 0920  
*	commit - changed CreateCvsFunction to include time span on .csv file  

2023 03 14 0445  
*	commit  - crashes when 'RecordAndDisplayTradesWithButtonsRyzen2' is placed on a chart with a playback connection and a trade is placed  
	removing sections to try to locate problem code  

2023 03 15 0140  
*	commit - made note in 'RecordAndDisplayTradesWithButtonsRyzen2' about first and last bar for .csv file name  
	database my be open and when trade is place there is a locked exception being thrown  

2023 03 19 1330  
*	commit - about to add change to 'SqLiteExecutionsToListAndQueryResults.Program' to use NTDrawline with attributes  

*	commit - about to add change to 'SqLiteExecutionsToListAndQueryResults.Program' to use NTDrawline with attributes  

2023 04 03  
*	commit - use '// Instead of Print()
	NinjaTrader.Code.Output.Process("my message", PrintTo.OutputTab1);' in AddOn Class  2023 04 03 0855  

2023 04 03 2103  
*	commit - 'about to add change to 'SqLiteExecutionsToListAndQueryResults.Program' to use NTDrawline with attributes' gets lines woking again  

2023 04 16 0300  
*	commit - works - can use data from TS to display in NT!!!!
	uses 'RecordAndDisplayTradesWithButtonsRyzen2' to call 'CreateCvsFunc' which calls 'SqLiteExecutionsToListAndQueryResults'
	a list is passed 
```
	            Parameters.Input parameters = new Parameters.Input()  
            {  
                BPlayback = bPlayback,  
                Name = Bars.Instrument.MasterInstrument.Name,  
                StartDate = StartTime.ToString(),  
                EndDate = EndTime.ToString(),  
                InputPath = @"Data Source = " + InputFile,   
                OutputPath = OutputFile,  
                TimeFirstBarOnChart = inputFirstBarOnChart,  
                TimeLastBarOnChart = inPutLastBarOnChart,  
            };  
            if (EnumValue == MyEnum.Playback)  
            {  
                parameters.BPlayback = true;  
            }  
```
    which is used in 'SqLiteExecutionsToListAndQueryResults' -> 'public static void main(Parameters.Input input)' -> 'Methods.getInstList' to read C:\Users\Rod\Documents\NinjaTrader 8\db\NinjaTrader.sqlite  
    the Executions table contains the entry and exit price/times  
    the symbol is supplies by the chart  
    the list created is then queried for all trades greater than the input starting date and account type (1 for Playback, 2 for simulated trades) 
    a new list is made - 'workingTrades'  
    workingTrades is used to fill in class source.Trades  
    source. has a number of properties and 3 lists Trades, Csv, and NTDrawLine  
    a number of extensions for source create source.NTDrawLine  
    source.NTDrawLine LinQtoCSV to write the list to NT usually csvNTDrawline.csv  
    this file is read in NT uses indicator - RecordAndDisplayTradesWithButtonsRyzen2, converted to lines and displayed on chart  using 'ReadCsvAndDrawLines()'  
    lines are now on chart  
    Data from TS is collected by creating a .csv file Export -> Trades List  
    The TradesList is placed in 'C:\Users\Rod\Cloud-Drive\TradeManagerAnalysis'  
    It is read by 'ReadAllLines From IDrive-Sync and Remove Header 1 DST.linq' or  
        'ReadAllLines From IDrive-Sync and Remove Header 1.linq'  
    LINQtoCSV is again used to create "C:\data\csvNTDrawline.csv"   
    this file is same format as the csvNTDrawline.csv which was created by 'RecordAndDisplayTradesWithButtonsRyzen2'  
    the indicator 'RecordAndDisplayTradesWithButtonsRyzen2' has input selection parameters  'OutputFile' is where the .csv file is placed which is read to creat chart lines.  
    that file can be given any location so using the linqPad .csv loacation C:\data can be read and plced on a chart  

    at the ppresent all of the extensions and classes in 'SqLiteExecutionsToListAndQueryResults' are compiled to .dlls and referenced in NT  
    project is underway to use .cs files in NT AddOns instead of .dlls for ease of acces when changeing and duplication flies for ZBook and Ryzen-1  
    'RecordAndDisplayTradesWithButtonsRyzen2' is source controlled in 'https://github.com/rmckay49/Custom-Ryzen-2.git'  

```

2023 05 14 1250  
	commit - all files for 'RecordAndDisplayTradesWithButtons' and AddOns are synced  

2023 05 27 1030  
    commit - branch AddPL.  Exception from Close[barsAgo] in print statement

2023 05 28 0935  
    commit - crude GetBar() working example  