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