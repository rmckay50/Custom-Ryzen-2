#region Comments
/*
 * 2022 12 15 1900  
 *	Added script in 'State.DataLoaded' to draw lines from 
 *	Changed hideDrawsFunc() to get IsAttachedToNinjaScript for each line - drawn by indicator
 * 
 * 2023 02 16 1145 
 *  adding 'ReadCsvAndDrawLines' which reads file from .csv draws the lines
 *  on load the files is read in State.Loaded
 *  
 * 2023 02 18 1225 
 *  transfer from 'ToolbarButtonsCopy'
 *  selecting a file for output other than 'csvNTDrawline.csv' allows display of prior trades
 *      selecting NQ Playback xxxx will show trades from that run
 *      
 * 2023 04 25 1200  
 *  'SqLiteExecutionsToListAndQueryResults' works correctly as .dll 
 *  attemptiong transfer to AddOns
*/
#endregion Comments

#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
using LINQtoCSV;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.Custom.AddOns;
using Trade = NinjaTrader.Custom.AddOns.Trade;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.My
{

    public class RecordAndDisplayTradesWithButtons : Indicator
    {

        private bool drawSwitch = true;
        private bool indiSwitch;
        private bool p_LSwitch = true;

        // Define a Chart object to refer to the chart on which the indicator resides
        private Chart chartWindow;

        // Define a Button
        private new System.Windows.Controls.Button btnTradeLines;
        private new System.Windows.Controls.Button btnP_L;
        private new System.Windows.Controls.Button btnUserDrawObjs;
        private new System.Windows.Controls.Button btnIndicators;
        private new System.Windows.Controls.Button btnShowTrades;
        private new System.Windows.Controls.Button btnHideWicks;
        private new System.Windows.Controls.Button btnCreateCsv;

        private bool IsToolBarButtonAdded;

        private IEnumerable<NTDrawLine> returnedClass;
        private DateTime inputFirstBarTime, inputLastBarTime;
        private string inputFirstBarOnChart, inPutLastBarOnChart;

        private ChartExecutionStyle startingExecutionStyle = ChartExecutionStyle.DoNotPlot;

        //  Get Username from Environment.UserName for InputFile and OutputFile initialization
        private string userName = Environment.UserName;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Enter the description for your new custom Indicator here.";
                Name = "RecordAndDisplayTradesWithButtons";
                Calculate = Calculate.OnBarClose;
                DisplayInDataBox = false;
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                IsAutoScale = true;
                IsSuspendedWhileInactive = true;
                indiSwitch = true;
                colorActiveCursor = true;
                ActiveCursorColor = Brushes.DarkGreen;
                InactiveCursorColor = Brushes.DimGray;
                PixelsAboveBelowBar = 50;
                IsSuspendedWhileInactive = true;
                StartTime = DateTime.Parse("06/01/ 2023");
                EndTime = DateTime.Parse("06/30/ 2023");
                EnumValue = MyEnum.Futures;
                //  The userName needs to be correct to keep ReadCsvAndDrawLines() in State.Historical from throwing exception
                InputFile = @"C:\Users\" + userName + @"\Documents\NinjaTrader 8\db\NinjaTrader.sqlite";
                OutputFile = @"C:\Users\" + userName + @"\Documents\NinjaTrader 8\csvNTDrawline.csv";
            }
            else if (State == State.Configure)
            {
                IsOverlay = false;
            }
            if (State == State.Historical)
            {
                ReadCsvAndDrawLines();
            }
            else if (State == State.Realtime)
            {
                //Call the custom method in State.Historical or State.Realtime to ensure it is only done when applied to a chart not when loaded in the Indicators window				
                if (ChartControl != null && !IsToolBarButtonAdded)
                {
                    ChartControl.Dispatcher.InvokeAsync((Action)(() => // Use this.Dispatcher to ensure code is executed on the proper thread
                    {
                        AddButtonToToolbar();
                    }));
                }
            }
            else if (State == State.Terminated)
            {
                if (chartWindow != null)
                {
                    ChartControl.Dispatcher.InvokeAsync((Action)(() => //Dispatcher used to Assure Executed on UI Thread
                    {
                        DisposeCleanUp();
                    }));
                }
            }
        }

        protected override void OnBarUpdate()
        {
        }

        #region Properties
        //	https://ninjatrader.com/support/forum/forum/ninjatrader-8/indicator-development/1050873-date-picker-property#post1050892
        [NinjaScriptProperty]
        [PropertyEditor("NinjaTrader.Gui.Tools.ChartAnchorTimeEditor")]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Start Time", GroupName = "Parameters", Order = 1)]

        public DateTime StartTime { get; set; }

        [NinjaScriptProperty]
        [PropertyEditor("NinjaTrader.Gui.Tools.ChartAnchorTimeEditor")]
        [Display(ResourceType = typeof(Custom.Resource), Name = "End Time", GroupName = "Parameters", Order = 2)]
        public DateTime EndTime { get; set; }

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

        #endregion Properties

        #region Use Case #4: Display "Friendly" enum values

        [TypeConverter(typeof(FriendlyEnumConverter))] // Converts the enum to string values
        [PropertyEditor("NinjaTrader.Gui.Tools.StringStandardValuesEditorKey")] // Enums normally automatically get a combo box, but we need to apply this specific editor so default value is automatically selected
        [Display(Name = "Switch", Order = 3, GroupName = "Parameters")]
        public MyEnum EnumValue
        { get; set; }

        #region Use Case #4: Display "friendly" enum values
        public enum MyEnum
        {
            Futures,
            Playback,
            Stocks
        }

        // Since this is only being applied to a specific property rather than the whole class,
        // we don't need to inherit from IndicatorBaseConverter and we can just use a generic TypeConverter
        public class FriendlyEnumConverter : TypeConverter
        {
            // Set the values to appear in the combo box
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                List<string> values = new List<string>() { "Futures", "Playback", "Stocks" };

                return new StandardValuesCollection(values);
            }

            // map the value from "Friendly" string to MyEnum type
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                string stringVal = value.ToString();

                switch (stringVal)
                {
                    case "Futures":
                        return MyEnum.Futures;
                    case "Playback":
                        return MyEnum.Playback;
                    case "Stocks":
                        return MyEnum.Stocks;
                }
                return MyEnum.Playback;
            }

            // map the MyEnum type to "Friendly" string
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                MyEnum stringVal = (MyEnum)Enum.Parse(typeof(MyEnum), value.ToString());

                switch (stringVal)
                {
                    case MyEnum.Futures:
                        return "Futures";
                    case MyEnum.Playback:
                        return "Playback";
                    case MyEnum.Stocks:
                        return "Stocks";
                }
                return string.Empty;
            }

            // required interface members needed to compile
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            { return true; }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            { return true; }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            { return true; }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            { return true; }
        }
        #endregion

        #endregion Properties

        private void AddButtonToToolbar()
        {
            //Obtain the Chart on which the indicator is configured
            chartWindow = Window.GetWindow(this.ChartControl.Parent) as Chart;
            if (chartWindow == null)
            {
                Print("chartWindow == null");
                return;
            }

            // subscribe chartwindow to keypress events
            if (chartWindow != null)
            {
                chartWindow.KeyUp += OnKeyUp;
                chartWindow.MouseLeftButtonDown += OnMouseLeftDown;
                chartWindow.PreviewMouseWheel += OnMouseWheel;
                chartWindow.MouseEnter += OnMouseEnter;
                chartWindow.MouseLeave += OnMouseLeave;
            }

            // Create a style to apply to the button
            Style btnStyle = new Style();
            btnStyle.TargetType = typeof(System.Windows.Controls.Button);

            btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.FontSizeProperty, 11.0));
            btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.FontFamilyProperty, new FontFamily("Arial")));
            btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.FontWeightProperty, FontWeights.Bold));
            btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.MarginProperty, new Thickness(2, 0, 2, 0)));
            btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.PaddingProperty, new Thickness(4, 2, 4, 2)));
            btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.ForegroundProperty, Brushes.WhiteSmoke));
            btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.BackgroundProperty, Brushes.DimGray));
            btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.IsEnabledProperty, true));
            btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.HorizontalAlignmentProperty, HorizontalAlignment.Center));

            // Instantiate the buttons
            btnUserDrawObjs = new System.Windows.Controls.Button();
            btnTradeLines = new System.Windows.Controls.Button();
            btnP_L = new System.Windows.Controls.Button();
            btnIndicators = new System.Windows.Controls.Button();
            btnShowTrades = new System.Windows.Controls.Button();
            btnHideWicks = new System.Windows.Controls.Button();
            btnCreateCsv = new System.Windows.Controls.Button();

            // Set button names
            btnTradeLines.Content = "Toggle Trade Lines";
            btnP_L.Content = "Toggle P/L";
            btnUserDrawObjs.Content = "Toggle Draw";
            btnIndicators.Content = "Toggle Indicators";
            btnShowTrades.Content = "Toggle Trades";
            btnHideWicks.Content = "Toggle Wicks";
            btnCreateCsv.Content = "Create Csv";

            // Set Button style            
            btnTradeLines.Style = btnStyle;
            btnP_L.Style = btnStyle;
            btnUserDrawObjs.Style = btnStyle;
            btnIndicators.Style = btnStyle;
            btnShowTrades.Style = btnStyle;
            btnHideWicks.Style = btnStyle;
            btnCreateCsv.Style = btnStyle;

            // Add the Buttons to the chart's toolbar
            chartWindow.MainMenu.Add(btnTradeLines);
            chartWindow.MainMenu.Add(btnP_L);
            chartWindow.MainMenu.Add(btnUserDrawObjs);
            chartWindow.MainMenu.Add(btnIndicators);
            chartWindow.MainMenu.Add(btnShowTrades);
            chartWindow.MainMenu.Add(btnHideWicks);
            chartWindow.MainMenu.Add(btnCreateCsv);

            // Set button visibility
            btnTradeLines.Visibility = Visibility.Visible;
            btnP_L.Visibility = Visibility.Visible;
            btnUserDrawObjs.Visibility = Visibility.Visible;
            btnIndicators.Visibility = Visibility.Visible;
            btnShowTrades.Visibility = Visibility.Visible;
            btnHideWicks.Visibility = Visibility.Visible;
            btnCreateCsv.Visibility = Visibility.Visible;

            // Subscribe to click events
            btnTradeLines.Click += btnTradeLinesClick;
            btnP_L.Click += btnP_LClick;
            btnUserDrawObjs.Click += btnUserDrawObjsClick;
            btnIndicators.Click += btnIndicatorsClick;
            btnShowTrades.Click += btnShowTradesClick;
            btnHideWicks.Click += btnHideWicksClick;
            btnCreateCsv.Click += btnCreateCsvClick;

            // Set this value to true so it doesn't add the
            // toolbar multiple times if NS code is refreshed
            IsToolBarButtonAdded = true;
        }

        protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
        {
        }

        //  Toggle trade lines
        private void btnTradeLinesClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                ReadCsvAndDrawLines();
                hideDrawsFunc();
            }
        }

        //  Toggle P/L

        private void btnP_LClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                //ReadCsvAndDrawLines();
                hideP_LFunc();
            }
        }


        //  Toggle user drawn objects
        private void btnUserDrawObjsClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                hideUserDrawsFunc();
            }
        }

        private void btnIndicatorsClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                // toggle all indicators
                foreach (var obj in chartWindow.ActiveChartControl.ChartObjects)
                {
                    var indi = obj as Indicator;
                    if (indi != null)
                    {
                        if (indiSwitch)
                        {
                            if (indi.Name != "ABMarketOpenLine")
                            {
                                indi.IsVisible = false;
                                //	Change btn background to Green
                                btnIndicators.Background = Brushes.Green;

                            }
                        }
                        else if (!indiSwitch)
                        {
                            if (indi.Name != "ABMarketOpenLine")
                            {
                                indi.IsVisible = true;
                                //	Change btn background to DimGray
                                btnIndicators.Background = Brushes.DimGray;

                            }
                        }
                    }
                }
                indiSwitch = !indiSwitch;
                chartWindow.ActiveChartControl.InvalidateVisual();
                ForceRefresh();
            }
        }

        private void btnShowTradesClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                // toggle trades				
                foreach (var obj in chartWindow.ActiveChartControl.ChartObjects)
                {
                    var trades = obj as ChartBars;
                    if (trades != null)
                    {
                        switch (trades.Properties.PlotExecutions)
                        {
                            case ChartExecutionStyle.DoNotPlot:
                                {
                                    if (startingExecutionStyle == ChartExecutionStyle.MarkersOnly)
                                    {
                                        trades.Properties.PlotExecutions = ChartExecutionStyle.MarkersOnly;
                                        btnShowTrades.Background = Brushes.DimGray;
                                        goto Done;
                                    }
                                    else if (startingExecutionStyle == ChartExecutionStyle.TextAndMarker)
                                    {
                                        trades.Properties.PlotExecutions = ChartExecutionStyle.TextAndMarker;
                                        btnShowTrades.Background = Brushes.DimGray;
                                        goto Done;
                                    }
                                    break;
                                }
                            case ChartExecutionStyle.MarkersOnly:
                                {
                                    startingExecutionStyle = ChartExecutionStyle.MarkersOnly;
                                    trades.Properties.PlotExecutions = ChartExecutionStyle.DoNotPlot;
                                    btnShowTrades.Background = Brushes.Green;
                                    break;
                                }
                            case ChartExecutionStyle.TextAndMarker:
                                {
                                    startingExecutionStyle = ChartExecutionStyle.TextAndMarker;
                                    trades.Properties.PlotExecutions = ChartExecutionStyle.DoNotPlot;
                                    btnShowTrades.Background = Brushes.Green;
                                    break;
                                }

                            Done:
                                continue;

                        }
                    }
                }
                chartWindow.ActiveChartControl.InvalidateVisual();
                ForceRefresh();
            }
        }

        private void btnHideWicksClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                hideWicksFunc();
            }
        }
        private void btnCreateCsvClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                CreateCvsFunc();
            }
        }

        private void ReadCsvAndDrawLines()
        {
            #region Use LINQtoCSV to read "csvNTDrawline.csv"
            CsvFileDescription scvDescript = new CsvFileDescription();
            CsvContext cc = new CsvContext();
            CsvFileDescription dataFromFile = new CsvFileDescription();

            //	Read in file 'C:\data\csvNTDrawline.csv'  Fills returnedClass
            returnedClass = cc.Read<NTDrawLine>
                            (
                                OutputFile,
                                dataFromFile
                            );
            #endregion Use LINQtoCSV to read "csvNTDrawline.csv"

            #region Define Brushes
            int i = 0;                                  //	Used for tag in Write.Line()
            string color = "";
            SolidColorBrush red = Brushes.Red;
            SolidColorBrush blue = Brushes.Blue;
            SolidColorBrush brush = Brushes.Blue;
            #endregion Define Brushes

            #region foreach() Through returnedClass, Draw.Line(), and Draw.Text()
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
                else if (rc.P_L >= 0 && rc.Long_Short == "Short")
                {
                    brush = Brushes.Blue;
                }
                // Long Loss
                else if (rc.P_L < 0 && rc.Long_Short == "Long")
                {
                    brush = Brushes.Red;
                }
                // Short Loss
                else if (rc.P_L < 0 && rc.Long_Short == "Short")
                {
                    brush = Brushes.Red;
                };
                #endregion Determine brush color

                #region Draw.Line()
                Draw.Line
                    (this,
                    i.ToString(),
                    false,
                    DateTime.Parse(rc.StartTime),
                    rc.StartY,
                    DateTime.Parse(rc.EndTime),
                    rc.EndY,
                    brush,
                    DashStyleHelper.Solid,
                    5);
                #endregion Draw.Line()

                #region Draw.Text()
                try
                {
                    TriggerCustomEvent(o =>
                    {
                        if (CurrentBar > 0)
                        {
                            //  get days on chart
                            //  needs to be greater than 1st day in C:\Users\Rod\Documents\NinjaTrader 8\csvNTDrawline.csv - from property 'OutputFile'
                            if (CurrentBar < 10) return;

                            //  first bar on chart
                            DateTime StartDate = Time[CurrentBar - 1];

                            //  last bar on chart
                            DateTime EndDate = Time[0];

                            //  days on chart - may need to use (d1 - d2).Days - Toatal days gives fraction of days
                            int daysOnChart = (int)(EndDate - StartDate).TotalDays;

                            //Print("Days on chart: " + daysOnChart.ToString());

                            var sTime = DateTime.Parse(rc.StartTime);

                            //  need DAteTime.Now to calculate days between now and trade date
                            var timeNow = DateTime.Now;

                            //  get number of days to trade
                            int daysAgo = (timeNow - sTime).Days;

                            //  number of days to trade needs to be less than the number of days on the chart
                            if (daysAgo < daysOnChart)
                            { 
                                //  instantiate outside of brackets
                                double hi = 0;
                                double lo = 0;
                                //  get high and low
                                if (daysAgo > 0)
                                {
                                    hi = Bars.GetDayBar(daysAgo).High;
                                    lo = Bars.GetDayBar(daysAgo).Low;
                                }
                                //var x = Bars.GetDayBar(daysAgo).Open;

                                //var y = CurrentDayOHL().CurrentLow[0];
                                //var nowTime = DateTime.Now;

                                var eTime = rc.EndTime;

                                if (rc.DailyTotal > 0)
                                {
                                    Print("\nrc.Endtime = " + rc.EndTime.ToString());
                                    Print(string.Format("Daily total is {0}", rc.DailyTotal.ToString("0.00")));
                                    var etDateOnly = DateTime.Parse(rc.EndTime);
                                    var etDateOnlySubString = rc.EndTime.Substring(9);
                                    //Print("etDateOnlySubString" + etDateOnlySubString);
                                    Print(string.Format("High = {0} Low = {1}", hi, lo));
                                    //Print(barsAgo.ToString());
                                    //Print("Number of Days: " + (timeNow - etDateOnly).Days);
                                    Print("Number of Days: " + (timeNow - sTime).Days + "\n");

                                }
                            }
                        }
                        //}
                    }, null);

            }
                        catch (Exception ex)
                        {
                            Print (ex.ToString());
                    // Submits an entry into the Control Center logs to inform the user of an error				
                    Log("SampleTryCatch Error: Please check your indicator for errors.", NinjaTrader.Cbi.LogLevel.Warning);

                }

                try
                        {
                    TriggerCustomEvent(o =>
                    {
                        var sTime = DateTime.Parse(rc.StartTime);

                        int barsAgo = CurrentBar - Bars.GetBar(sTime);
                        //  get chart font for Draw.Text()
                        var chartControl = ChartControl.Properties.LabelFont;
                        //var pixels 
                        SimpleFont chartFont = chartControl;


                        if (CurrentBar > 0 && CurrentBar > barsAgo)
                        {
                            //Print("The prior trading day's close is: " + Bars.GetDayBar(1).Close);



                            // Print P/L in blue below line start
                            if (rc.P_L >= 0)
                            {
                                Draw.Text(this, i.ToString() + "Text", false, rc.P_L.ToString(), sTime, rc.StartY, -PixelsAboveBelowBar, Brushes.Blue, chartFont, TextAlignment.Center, Brushes.White, Brushes.White, 100);

                            }
                            // Print P/L in red above line start
                            else
                            {
                                Draw.Text(this, i.ToString() + "Text", false, rc.P_L.ToString(), sTime, rc.StartY, PixelsAboveBelowBar, Brushes.Red, chartFont, TextAlignment.Center, Brushes.White, Brushes.White, 100);
                            }

                            //  if DailyTotal is available draw it at midpoint of day
                            //if (rc.DailyTotal != 0)
                            //{
                                //var noon = sTime.Date.AddHours(12);
                            //}
                       }


                    }, null);
               }
                catch (Exception ex)
                {
                    Print(ex);
                    // Prints the caught exception in the Output Window
                    Print(Time[0] + " " + ex.ToString());

                }
                #endregion Draw.Text()

                i++;
            }
            #endregion 
        }
        private void CreateCvsFunc()
        {
            var bPlayback = false;
            //  get first and last bar on chart
            //   use these when giving the .csv file a name in 
            inputFirstBarTime = ChartBars.GetTimeByBarIdx(ChartControl, ChartBars.FromIndex);
            inputFirstBarOnChart = inputFirstBarTime.ToString("yy MM dd HH_mm");
            inputLastBarTime = ChartBars.GetTimeByBarIdx(ChartControl, ChartBars.ToIndex);
            inPutLastBarOnChart = inputLastBarTime.ToString("yy MM dd HH_mm");
            //  Convert DateTime Instrument.Expiry to long 
            var expiryAsDateTime = Convert.ToDateTime(Instrument.Expiry);
            long expiry = expiryAsDateTime.Ticks;

            Input parameters = new Input()
            {
                BPlayback = bPlayback,
                Name = Bars.Instrument.MasterInstrument.Name,
                StartDate = StartTime.ToString(),
                EndDate = EndTime.ToString(),
                InputPath = @"Data Source = " + InputFile,
                OutputPath = OutputFile,
                TimeFirstBarOnChart = inputFirstBarOnChart,
                TimeLastBarOnChart = inPutLastBarOnChart,
                Expiry = expiry
            };
            if (EnumValue == MyEnum.Playback)
            {
                parameters.BPlayback = true;
            }

            List<CSV> CSv = new List<CSV>();
            //  list to hold valiables in Executions table from NinjaTrader.sqlite
            List<Executions> listExecution = new List<Executions>();
            List<NTDrawLine> nTDrawline = new List<NTDrawLine>();
            //  list to hold Ret() format from listExecution
            List<Ret> listExecutionRet = new List<Ret>();
            Source source = new Source();
            List<Trade> trades = new List<Trade>();

            //  Call main()
            SqLiteExecutionsToListAndQueryResults.Program.main(parameters);
        }

        private void hideDrawsFunc()
        {
//            ClearOutputWindow();
            // Sets drawSwitch based on whether there are any drawings on the chart
            foreach (DrawingTool dTL in DrawObjects.ToList())
            {
                //  'break' kicks execution out of loop
                if (dTL.IsAttachedToNinjaScript)
                {
                    if (dTL.IsVisible && dTL.IsAttachedToNinjaScript)
                    {
                        drawSwitch = true;
                        btnTradeLines.Background = Brushes.Green;
                        break;
                    }
                    else
                    {
                        drawSwitch = false;
                        btnTradeLines.Background = Brushes.DimGray;
                        break;
                    }
                }

            }

            foreach (DrawingTool dTL in DrawObjects.ToList())
            {
                if (dTL.IsAttachedToNinjaScript)
                {
                    if (drawSwitch)
                    {
                        if (dTL.Tag.Contains("Text"))
                        {
                        }
                        dTL.IsVisible = false;
                    }
                    else if (!drawSwitch)
                    {
                        dTL.IsVisible = true;
                    }
                }

            }

            drawSwitch = !drawSwitch;
            chartWindow.ActiveChartControl.InvalidateVisual();
            ForceRefresh();
        }
        //  toggle P/L
        private void hideP_LFunc()
        {
//            ClearOutputWindow();

            foreach (DrawingTool dTL in DrawObjects.ToList())
            {
                //  Need to check that drawing object is P/L - first was the line
                if (dTL.IsAttachedToNinjaScript && dTL.Tag.Contains("Text"))
                {
                    if (dTL.IsVisible && dTL.IsAttachedToNinjaScript)
                    {
                        //  p_LSwitch set to true when P/L is not showing and brush is set to Green
                        p_LSwitch = true;
                        btnP_L.Background = Brushes.Green;
                        break;
                    }
                    else
                    {
                        //  p_LSwitch set to false when P/L is showing and brush is set to DimGray
                        p_LSwitch = false;
                        btnP_L.Background = Brushes.DimGray;
                        break;
                    }
                }
            }

            try
            {
                foreach (DrawingTool dTL in DrawObjects.ToList())
                {
                    if (dTL.IsAttachedToNinjaScript)
                    {
                        if (p_LSwitch)
                        {
                            if (dTL.Tag.Contains("Text"))
                            {
                                dTL.IsVisible = false;
                            }
                        }
                        else if (!p_LSwitch)
                        {
                            if (dTL.Tag.Contains("Text"))
                            {
                                dTL.IsVisible = true;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Print(ex);
            }
            p_LSwitch = !p_LSwitch;
            chartWindow.ActiveChartControl.InvalidateVisual();
            ForceRefresh();
        }


        private void hideUserDrawsFunc()
        {
            // turns off historical drawings but future drawings will show until hidden

            // Sets drawSwitch based on whether there are any drawings on the chart
            //foreach (var obj in chartWindow.ActiveChartControl.ChartObjects)
            foreach (DrawingTool dTL in DrawObjects.ToList())
            {
                var draw = dTL as DrawingTool;
                if (draw != null)
                {
                    if (draw.IsVisible && draw.IsUserDrawn)
                    {
                        drawSwitch = true;
                        btnUserDrawObjs.Background = Brushes.Green;
                        break;
                    }
                    else
                    {
                        drawSwitch = false;
                        btnUserDrawObjs.Background = Brushes.DimGray;
                    }
                }
            }

            //foreach (var obj in chartWindow.ActiveChartControl.ChartObjects)
            foreach (DrawingTool dTL in DrawObjects.ToList())
            {
                //var draw = obj as DrawingTool;
                var draw = dTL as DrawingTool;
                if (draw != null)
                {
                    if (draw.IsUserDrawn)
                    {
                        if (drawSwitch)
                        {
                            draw.IsVisible = false;
                        }
                        else if (!drawSwitch)
                        {
                            draw.IsVisible = true;
                        }
                    }
                }
            }

            drawSwitch = !drawSwitch;
            chartWindow.ActiveChartControl.InvalidateVisual();
            ForceRefresh();
        }

        private void hideSelectedDrawFunc()
        {
            // turns off historical drawings but future drawings will show until hidden
            foreach (var obj in chartWindow.ActiveChartControl.ChartObjects)
            {
                var draw = obj as DrawingTool;
                if (draw != null)
                {
                    if (draw.IsUserDrawn && draw.IsSelected)
                    {
                        draw.IsVisible = false;  // hide selected drawing
                    }
                }
            }

            chartWindow.ActiveChartControl.InvalidateVisual();
            ForceRefresh();
        }

        private void hideWicksFunc()
        {
            // toggle wicks
            foreach (var obj in chartWindow.ActiveChartControl.ChartObjects)
            {
                var wicks = obj as ChartBars;
                if (wicks != null)
                {
                    switch (wicks.Properties.ChartStyle.Stroke2.Brush.ToString())
                    {
                        case "#FFFFFFFF":
                            {
                                wicks.Properties.ChartStyle.Stroke2.Brush = Brushes.Black;
                                btnHideWicks.Background = Brushes.Green;
                                break;
                            }

                        default:
                            {
                                wicks.Properties.ChartStyle.Stroke2.Brush = Brushes.White;
                                btnHideWicks.Background = Brushes.DimGray;
                                break;
                            }
                    }
                }
            }

            chartWindow.ActiveChartControl.InvalidateVisual();
            ForceRefresh();
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Oem3 && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) hideWicksFunc();
            if (e.Key == Key.D && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) hideDrawsFunc();
            if (e.Key == Key.H && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) hideSelectedDrawFunc();
        }

        public void OnMouseLeftDown(object sender, MouseEventArgs e)
        {
            // Hide selected drawing if CTRL + SHIFT + CLICK on a drawing
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))))
            {
                foreach (var obj in chartWindow.ActiveChartControl.ChartObjects)
                {
                    var draw = obj as DrawingTool;
                    if (draw != null)
                    {
                        if (draw.IsUserDrawn && draw.IsSelected)
                        {
                            draw.IsVisible = false;  // hide selected drawing
                        }
                    }
                }

                chartWindow.ActiveChartControl.InvalidateVisual();
                ForceRefresh();
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (chartWindow.ActiveChartControl != null && ChartBars != null)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (e.Delta < 0)
                    {
                        chartWindow.ActiveChartControl.Properties.BarDistance = (float)(chartWindow.ActiveChartControl.Properties.BarDistance * 0.9);
                        chartWindow.ActiveChartControl.BarWidth = chartWindow.ActiveChartControl.BarWidth * 0.9;
                    }
                    else if (e.Delta > 0)
                    {
                        chartWindow.ActiveChartControl.Properties.BarDistance = (float)(chartWindow.ActiveChartControl.Properties.BarDistance / 0.9);
                        chartWindow.ActiveChartControl.BarWidth = chartWindow.ActiveChartControl.BarWidth / 0.9;
                    }
                    e.Handled = true;
                    chartWindow.ActiveChartControl.InvalidateVisual();
                    ForceRefresh();
                }
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (chartWindow != null && chartWindow.ActiveChartControl.GetType() == typeof(ChartControl) && colorActiveCursor)
            {
                chartWindow.ActiveChartControl.Properties.CrosshairLabelBackground = ActiveCursorColor;
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (chartWindow != null && chartWindow.ActiveChartControl.GetType() == typeof(ChartControl) && colorActiveCursor)
            {
                chartWindow.ActiveChartControl.Properties.CrosshairLabelBackground = InactiveCursorColor;
            }
        }

        private void DisposeCleanUp()
        {
            // remove toolbar items and unsubscribe from events
            chartWindow.KeyUp -= OnKeyUp;
            chartWindow.MouseLeftButtonDown -= OnMouseLeftDown;
            chartWindow.PreviewMouseWheel -= OnMouseWheel;
            chartWindow.MouseEnter -= OnMouseEnter;
            chartWindow.MouseLeave -= OnMouseLeave;

            if (btnTradeLines != null) chartWindow.MainMenu.Remove(btnTradeLines);
            btnTradeLines.Click -= btnTradeLinesClick;
            if (btnP_L != null) chartWindow.MainMenu.Remove(btnP_L);
            btnP_L.Click -= btnP_LClick;
            if (btnUserDrawObjs != null) chartWindow.MainMenu.Remove(btnUserDrawObjs);
            btnUserDrawObjs.Click -= btnUserDrawObjsClick;
            if (btnIndicators != null) chartWindow.MainMenu.Remove(btnIndicators);
            btnIndicators.Click -= btnIndicatorsClick;
            if (btnShowTrades != null) chartWindow.MainMenu.Remove(btnShowTrades);
            btnShowTrades.Click -= btnShowTradesClick;
            if (btnHideWicks != null) chartWindow.MainMenu.Remove(btnHideWicks);
            btnHideWicks.Click -= btnHideWicksClick;
            if (btnCreateCsv != null) chartWindow.MainMenu.Remove(btnCreateCsv);
            btnCreateCsv.Click -= btnCreateCsvClick;

        }

        #region Properties
        [NinjaScriptProperty]
        [Display(Name = "Color active cursor labels?", Order = 0, GroupName = "Other Settings")]
        public bool colorActiveCursor
        { get; set; }

        [XmlIgnore]
        [Display(Name = "Active Cursor Color", Order = 1, GroupName = "Other Settings")]
        public Brush ActiveCursorColor
        { get; set; }

        [Browsable(false)]
        public string ActiveCursorColorSerializable
        {
            get { return Serialize.BrushToString(ActiveCursorColor); }
            set { ActiveCursorColor = Serialize.StringToBrush(value); }
        }

        [XmlIgnore]
        [Display(Name = "Inctive Cursor Color", Order = 2, GroupName = "Other Settings")]
        public Brush InactiveCursorColor
        { get; set; }

        [Browsable(false)]
        public string InactiveCursorColorSerializable
        {
            get { return Serialize.BrushToString(InactiveCursorColor); }
            set { InactiveCursorColor = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [Range(0, int.MaxValue)]
        [Display(Name = "Pixels Above/Below Bar", Description = "Number of Pixels above and below bars", Order = 3, GroupName = "Other Settings")]
        public int PixelsAboveBelowBar
        { get; set; }

        #endregion

    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private My.RecordAndDisplayTradesWithButtons[] cacheRecordAndDisplayTradesWithButtons;
		public My.RecordAndDisplayTradesWithButtons RecordAndDisplayTradesWithButtons(DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor, int pixelsAboveBelowBar)
		{
			return RecordAndDisplayTradesWithButtons(Input, startTime, endTime, inputFile, outputFile, colorActiveCursor, pixelsAboveBelowBar);
		}

		public My.RecordAndDisplayTradesWithButtons RecordAndDisplayTradesWithButtons(ISeries<double> input, DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor, int pixelsAboveBelowBar)
		{
			if (cacheRecordAndDisplayTradesWithButtons != null)
				for (int idx = 0; idx < cacheRecordAndDisplayTradesWithButtons.Length; idx++)
					if (cacheRecordAndDisplayTradesWithButtons[idx] != null && cacheRecordAndDisplayTradesWithButtons[idx].StartTime == startTime && cacheRecordAndDisplayTradesWithButtons[idx].EndTime == endTime && cacheRecordAndDisplayTradesWithButtons[idx].InputFile == inputFile && cacheRecordAndDisplayTradesWithButtons[idx].OutputFile == outputFile && cacheRecordAndDisplayTradesWithButtons[idx].colorActiveCursor == colorActiveCursor && cacheRecordAndDisplayTradesWithButtons[idx].PixelsAboveBelowBar == pixelsAboveBelowBar && cacheRecordAndDisplayTradesWithButtons[idx].EqualsInput(input))
						return cacheRecordAndDisplayTradesWithButtons[idx];
			return CacheIndicator<My.RecordAndDisplayTradesWithButtons>(new My.RecordAndDisplayTradesWithButtons(){ StartTime = startTime, EndTime = endTime, InputFile = inputFile, OutputFile = outputFile, colorActiveCursor = colorActiveCursor, PixelsAboveBelowBar = pixelsAboveBelowBar }, input, ref cacheRecordAndDisplayTradesWithButtons);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.My.RecordAndDisplayTradesWithButtons RecordAndDisplayTradesWithButtons(DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor, int pixelsAboveBelowBar)
		{
			return indicator.RecordAndDisplayTradesWithButtons(Input, startTime, endTime, inputFile, outputFile, colorActiveCursor, pixelsAboveBelowBar);
		}

		public Indicators.My.RecordAndDisplayTradesWithButtons RecordAndDisplayTradesWithButtons(ISeries<double> input , DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor, int pixelsAboveBelowBar)
		{
			return indicator.RecordAndDisplayTradesWithButtons(input, startTime, endTime, inputFile, outputFile, colorActiveCursor, pixelsAboveBelowBar);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.My.RecordAndDisplayTradesWithButtons RecordAndDisplayTradesWithButtons(DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor, int pixelsAboveBelowBar)
		{
			return indicator.RecordAndDisplayTradesWithButtons(Input, startTime, endTime, inputFile, outputFile, colorActiveCursor, pixelsAboveBelowBar);
		}

		public Indicators.My.RecordAndDisplayTradesWithButtons RecordAndDisplayTradesWithButtons(ISeries<double> input , DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor, int pixelsAboveBelowBar)
		{
			return indicator.RecordAndDisplayTradesWithButtons(input, startTime, endTime, inputFile, outputFile, colorActiveCursor, pixelsAboveBelowBar);
		}
	}
}

#endregion
