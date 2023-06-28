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
using SharpDX.DirectWrite;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.My
{
    public class RecordAndDisplayTradesWithButtonsRyzen2 : Indicator
    {
        #region Variables
        private bool drawSwitch;
        private bool indiSwitch;

        // Define a Chart object to refer to the chart on which the indicator resides
        private Chart chartWindow;

        // Define a Button
        private new System.Windows.Controls.Button btnTradeLines;
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
        private bool _textIsBelowBars = true;
        private int _fontSize = 14;
        private int _pixelsAboveBelowBar = -50;
        private Brush _textColorDefinedbyUser = Brushes.Gray;
        #endregion Variables
        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Enter the description for your new custom Indicator here.";
                Name = "RecordAndDisplayTradesWithButtons";
                Calculate = Calculate.OnBarClose;
                DisplayInDataBox = false;
                //ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                ScaleJustification = ScaleJustification.Right;
                IsAutoScale = true;
                IsSuspendedWhileInactive = true;
                indiSwitch = true;
                colorActiveCursor = true;
                ActiveCursorColor = Brushes.DarkGreen;
                InactiveCursorColor = Brushes.DimGray;
                IsSuspendedWhileInactive = true;
                StartTime = DateTime.Parse("05/01/ 2023");
                EndTime = DateTime.Parse("05/30/ 2023");
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
            btnStyle.Setters.Add(new Setter(System.Windows.Controls.Button.FontFamilyProperty, new System.Windows.Media.FontFamily("Arial")));
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
            btnIndicators = new System.Windows.Controls.Button();
            btnShowTrades = new System.Windows.Controls.Button();
            btnHideWicks = new System.Windows.Controls.Button();
            btnCreateCsv = new System.Windows.Controls.Button();

            // Set button names
            btnTradeLines.Content = "Toggle Trade Lines";
            btnUserDrawObjs.Content = "Toggle Draw";
            btnIndicators.Content = "Toggle Indicators";
            btnShowTrades.Content = "Toggle Trades";
            btnHideWicks.Content = "Toggle Wicks";
            btnCreateCsv.Content = "Create Csv";

            // Set Button style            
            btnTradeLines.Style = btnStyle;
            btnUserDrawObjs.Style = btnStyle;
            btnIndicators.Style = btnStyle;
            btnShowTrades.Style = btnStyle;
            btnHideWicks.Style = btnStyle;
            btnCreateCsv.Style = btnStyle;

            // Add the Buttons to the chart's toolbar
            chartWindow.MainMenu.Add(btnTradeLines);
            chartWindow.MainMenu.Add(btnUserDrawObjs);
            chartWindow.MainMenu.Add(btnIndicators);
            chartWindow.MainMenu.Add(btnShowTrades);
            chartWindow.MainMenu.Add(btnHideWicks);
            chartWindow.MainMenu.Add(btnCreateCsv);

            // Set button visibility
            btnTradeLines.Visibility = Visibility.Visible;
            btnUserDrawObjs.Visibility = Visibility.Visible;
            btnIndicators.Visibility = Visibility.Visible;
            btnShowTrades.Visibility = Visibility.Visible;
            btnHideWicks.Visibility = Visibility.Visible;
            btnCreateCsv.Visibility = Visibility.Visible;

            // Subscribe to click events
            btnTradeLines.Click += btnTradeLinesClick;
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

        private void btnTradeLinesClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                ReadCsvAndDrawLines();
                hideDrawsFunc();
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
                //  used for saving plot style so that is can be change back to original
                //var startingExecutionStyle = ChartExecutionStyle.DoNotPlot;
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
                DateTime st = DateTime.Parse(rc.StartTime);
                //try 
                //{
                //int barsAgo = Bars.GetBar(st);

                var sTime = DateTime.Parse(rc.StartTime);
                int barsAgo = CurrentBar - Bars.GetBar(sTime);
                if (barsAgo > 0)
                {
                    var _textYStartingPoint = Low[barsAgo];
                    //NinjaTrader.Gui.Tools.SimpleFont myFont = new NinjaTrader.Gui.Tools.SimpleFont("Courier New", 14) { Size = _fontSize, Bold = false };
                    SimpleFont myFont = new SimpleFont("Courier New", 14) { Size = _fontSize, Bold = false }; ;

                    //Draw.Text(this, CurrentBar.ToString() + "Text", true, rc.P_L.ToString(), st, _textYStartingPoint, _pixelsAboveBelowBar, _textColorDefinedbyUser, myFont, System.Windows.TextAlignment.Center, null, null, 1);
                    var tag = rc.P_L.ToString();
                    var text = rc.P_L.ToString();
                    var startPt = _textYStartingPoint;
                    var pixels = _pixelsAboveBelowBar;
                    Draw.Text(this, CurrentBar.ToString() + "Text", true, rc.P_L.ToString(), barsAgo, _textYStartingPoint, _pixelsAboveBelowBar, _textColorDefinedbyUser, myFont, System.Windows.TextAlignment.Center, null, null, 1);

                    Draw.Text(this, CurrentBar.ToString() + "P/L", "6.7", barsAgo, Low[barsAgo]);
                    //Print(String.Format("ToTime(Time[0]) is {0} ToTime((rc.StartTime)) is {1} ", ToTime(Time[0]), ToTime(DateTime.Parse(rc.StartTime))));
                    // Print(String.Format("ToTime(Time[0]) is {0} ToTime((st)) is {1} ", ToTime(Time[0]), ToTime((st))));


                    //      for (int j = 0; j <= CurrentBar; j++)
                    //     {
                    ////if (ToTime(Time[0]) == ToTime(DateTime.Parse(rc.StartTime)))
                    // if (Time[j] <= st)
                    // {
                    //         Print(String.Format("CurrentBar is {0} {1} {2} {3}", CurrentBar.ToString(), Time[j].ToString(), Close[j], rc.StartTime));

                    //         //var st = DateTime.Parse("08/18/2018 07:22:16");
                    //         int barsAgo = CurrentBar - Bars.GetBar(DateTime.Parse(rc.StartTime));
                    //         if (barsAgo > 0)
                    //         {
                    //             Print(String.Format("CurrentBar is {0} barsAgo is {1}", CurrentBar.ToString(), barsAgo.ToString()));

                    //             // Print out the 9 AM bar closing price
                    //             Print("The close price on the 9 AM bar was: " + Close[barsAgo].ToString());
                    //             //firstPass = false;
                    //         }
                    //     }
                    // }
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.ToString());
                    //}
                }
                #endregion Draw.Text()

                i++;
            }
            #endregion foreach() Through returnedClass and Draw line


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

            Input parameters = new Input()
            {
                BPlayback = bPlayback,
                Name = Bars.Instrument.MasterInstrument.Name,
                StartDate = StartTime.ToString(),
                EndDate = EndTime.ToString(),
                //Path = @"Data Source = " + InputFile
                InputPath = @"Data Source = " + InputFile,
                OutputPath = OutputFile,
                TimeFirstBarOnChart = inputFirstBarOnChart,
                TimeLastBarOnChart = inPutLastBarOnChart,
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
            // toggle all drawing objects
            // turns off historical drawings but future drawings will show until hidden

            // Sets drawSwitch based on whether there are any drawings on the chart
            #region From ABToolBars in Ryzen-2

            // Sets drawSwitch based on whether there are any drawings on the chart
            foreach (DrawingTool dTL in DrawObjects.ToList())
            {
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
                    }
                }
            }

            foreach (DrawingTool dTL in DrawObjects.ToList())
            {
                //var draw = obj as DrawingTool;
                if (dTL.IsAttachedToNinjaScript)
                {
                    if (drawSwitch)
                    {
                        dTL.IsVisible = false;
                        // Print(draw.Name + " '" + draw.Tag + "' is hidden.");
                    }
                    else if (!drawSwitch)
                    {
                        dTL.IsVisible = true;
                        // Print(draw.Name + " '" + draw.Tag + "'  is visible.");
                    }
                }
            }



            #endregion From ABToolBars in Ryzen-2

            drawSwitch = !drawSwitch;
            chartWindow.ActiveChartControl.InvalidateVisual();
            ForceRefresh();
        }

        private void hideUserDrawsFunc()
        {
            // toggle all drawing objects
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
                            //btnUserDrawObjs.Background = Brushes.Green;
                            // Print(draw.Name + " '" + draw.Tag + "' is hidden.");
                        }
                        else if (!drawSwitch)
                        {
                            draw.IsVisible = true;
                            // Print(draw.Name + " '" + draw.Tag + "'  is visible.");
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

                        //case "#00FFFFFF":
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
        #endregion

    }
}

#region Drawing tool not used
//foreach (var obj in chartWindow.ActiveChartControl.ChartObjects)
//{
//             foreach (DrawingTool dTL in DrawObjects.ToList())
//	{
//		if (dTL.IsVisible && dTL.IsAttachedToNinjaScript)
//		//	RemoveDrawObject(dTL.Tag);
//		//Print(String.Format(dTL.Tag));
//                 if (drawSwitch)
//                 {
//                     dTL.IsVisible = false;
//                     // Print(draw.Name + " '" + draw.Tag + "' is hidden.");
//                 }
//                 else if (!drawSwitch)
//                 {
//                     dTL.IsVisible = true;
//                     // Print(draw.Name + " '" + draw.Tag + "'  is visible.");
//                 }

//             }
//             var draw = obj as DrawingTool;
//    if (dtl != null)
//    {
//        //if (draw.IsVisible && draw.IsUserDrawn)
//                     if (draw.IsVisible)

//                     {
//			Print(String.Format("draw.IsVisible is {0}", draw.IsVisible));
//                         drawSwitch = true;
//			break;
//		}
//		else
//		{
//			drawSwitch = false;
//		}
//    }
//}

//foreach (var obj in chartWindow.ActiveChartControl.ChartObjects)
//{
//    var draw = obj as DrawingTool;
//    if (draw != null)
//    {
//        if (draw.IsUserDrawn)
//        {						
//			if (drawSwitch)green
//			{
//				draw.IsVisible = false;
//				// Print(draw.Name + " '" + draw.Tag + "' is hidden.");
//			}
//			else if (!drawSwitch)
//			{
//				draw.IsVisible = true;
//				// Print(draw.Name + " '" + draw.Tag + "'  is visible.");
//			}
//        }
//    }
//}
#endregion Drawing tool not used

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
    public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
    {
        private My.RecordAndDisplayTradesWithButtonsRyzen2[] cacheRecordAndDisplayTradesWithButtonsRyzen2;
        public My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor)
        {
            return RecordAndDisplayTradesWithButtonsRyzen2(Input, startTime, endTime, inputFile, outputFile, colorActiveCursor);
        }

        public My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(ISeries<double> input, DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor)
        {
            if (cacheRecordAndDisplayTradesWithButtonsRyzen2 != null)
                for (int idx = 0; idx < cacheRecordAndDisplayTradesWithButtonsRyzen2.Length; idx++)
                    if (cacheRecordAndDisplayTradesWithButtonsRyzen2[idx] != null && cacheRecordAndDisplayTradesWithButtonsRyzen2[idx].StartTime == startTime && cacheRecordAndDisplayTradesWithButtonsRyzen2[idx].EndTime == endTime && cacheRecordAndDisplayTradesWithButtonsRyzen2[idx].InputFile == inputFile && cacheRecordAndDisplayTradesWithButtonsRyzen2[idx].OutputFile == outputFile && cacheRecordAndDisplayTradesWithButtonsRyzen2[idx].colorActiveCursor == colorActiveCursor && cacheRecordAndDisplayTradesWithButtonsRyzen2[idx].EqualsInput(input))
                        return cacheRecordAndDisplayTradesWithButtonsRyzen2[idx];
            return CacheIndicator<My.RecordAndDisplayTradesWithButtonsRyzen2>(new My.RecordAndDisplayTradesWithButtonsRyzen2() { StartTime = startTime, EndTime = endTime, InputFile = inputFile, OutputFile = outputFile, colorActiveCursor = colorActiveCursor }, input, ref cacheRecordAndDisplayTradesWithButtonsRyzen2);
        }
    }
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
    public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
    {
        public Indicators.My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor)
        {
            return indicator.RecordAndDisplayTradesWithButtonsRyzen2(Input, startTime, endTime, inputFile, outputFile, colorActiveCursor);
        }

        public Indicators.My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(ISeries<double> input, DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor)
        {
            return indicator.RecordAndDisplayTradesWithButtonsRyzen2(input, startTime, endTime, inputFile, outputFile, colorActiveCursor);
        }
    }
}

namespace NinjaTrader.NinjaScript.Strategies
{
    public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
    {
        public Indicators.My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor)
        {
            return indicator.RecordAndDisplayTradesWithButtonsRyzen2(Input, startTime, endTime, inputFile, outputFile, colorActiveCursor);
        }

        public Indicators.My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(ISeries<double> input, DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor)
        {
            return indicator.RecordAndDisplayTradesWithButtonsRyzen2(input, startTime, endTime, inputFile, outputFile, colorActiveCursor);
        }
    }
}

#endregion
