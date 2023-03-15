#region Comments
/*
 * 2023 02 19 0410
 *	Using new git branch to try to get 'RecordAndDisplayTradesWithButtonsRyzen2' working
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

using Parameters;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.My
{
	public class RecordAndDisplayTradesWithButtonsRyzen2 : Indicator
	{
        private bool drawSwitch;
        private bool indiSwitch;

        // Define a Chart object to refer to the chart on which the indicator resides
        private Chart chartWindow;

        // Define a Button
        private new System.Windows.Controls.Button btnTradeLines;
        private new System.Windows.Controls.Button btnIndicators;
        private new System.Windows.Controls.Button btnShowTrades;
        private new System.Windows.Controls.Button btnHideWicks;
        private new System.Windows.Controls.Button btnCreateCsv;


        private bool IsToolBarButtonAdded;

        private IEnumerable<NinjaTrader.Custom.AddOns.NTDrawLine> returnedClass;
        private DateTime inputFirstBarTime, inputLastBarTime;
        private string inputFirstBarOnChart, inPutLastBarOnChart;


        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
                Description = @"Enter the description for your new custom Indicator here.";
                Name = "RecordAndDisplayTradesWithButtonsRyzen2";
                Calculate = Calculate.OnBarClose;
                IsOverlay = true;
                DisplayInDataBox = false;
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                IsAutoScale = true;
                IsSuspendedWhileInactive = true;
                indiSwitch = true;
                colorActiveCursor = true;
                ActiveCursorColor = Brushes.DarkGreen;
                InactiveCursorColor = Brushes.DimGray;
                //	Used to retreive 'C:\data\csvNTDrawline.csv' 
                //	Will be used to redraw lines
                //IEnumerable<NinjaTrader.Custom.AddOns.NTDrawLine> returnedClass; 
                //Disable this property if your indicator requires custom values that cumulate with each new market data event. 
                //See Help Guide for additional information.
                IsSuspendedWhileInactive = true;
                StartTime = DateTime.Parse("01/01/2023");
                EndTime = DateTime.Parse("02/28/2023");
                EnumValue = MyEnum.Futures;
                InputFile = @"C:\Users\Rod\Documents\NinjaTrader 8\db\NinjaTrader.sqlite";
                OutputFile = @"C:\Users\Rod\Documents\NinjaTrader 8\csvNTDrawline.csv";
                


            }
			else if (State == State.Configure)
			{
			}

            ///<summary>
            /// Reads 'C:\data\csvNTDrawline.csv' and draws lines on chart
            /// </summary>

            else if (State == State.Historical)
            {
//                ReadCsvAndDrawLines();
            }

			else if (State == State.Transition)
			{
				///<summary>
				/// Try 'ReadCsvAndDrawLines()' here
				/// </summary>

				//ReadCsvAndDrawLines();
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
            //RemoveDrawObjects();
            //if (CurrentBar < 20) return;
            var s = StartTime;
            var e = EndTime;
            var i = InputFile;
            var o = OutputFile;
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
            btnTradeLines = new System.Windows.Controls.Button();
            btnIndicators = new System.Windows.Controls.Button();
            btnShowTrades = new System.Windows.Controls.Button();
            btnHideWicks = new System.Windows.Controls.Button();
            btnCreateCsv = new System.Windows.Controls.Button();

            // Set button names
            btnTradeLines.Content = "Toggle Trade Lines";
            btnIndicators.Content = "Toggle Indicators";
            btnShowTrades.Content = "Toggle Trades";
            btnHideWicks.Content = "Toggle Wicks";
            btnCreateCsv.Content = "Create Csv";

            // Set Button style            
            btnTradeLines.Style = btnStyle;
            btnIndicators.Style = btnStyle;
            btnShowTrades.Style = btnStyle;
            btnHideWicks.Style = btnStyle;
            btnCreateCsv.Style = btnStyle;

            // Add the Buttons to the chart's toolbar
            chartWindow.MainMenu.Add(btnTradeLines);
            chartWindow.MainMenu.Add(btnIndicators);
            chartWindow.MainMenu.Add(btnShowTrades);
            chartWindow.MainMenu.Add(btnHideWicks);
            chartWindow.MainMenu.Add(btnCreateCsv);

            // Set button visibility
            btnTradeLines.Visibility = Visibility.Visible;
            btnIndicators.Visibility = Visibility.Visible;
            btnShowTrades.Visibility = Visibility.Visible;
            btnHideWicks.Visibility = Visibility.Visible;
            btnCreateCsv.Visibility = Visibility.Visible;

            // Subscribe to click events
            btnTradeLines.Click += btnTradeLinesClick;
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
                                    trades.Properties.PlotExecutions = ChartExecutionStyle.MarkersOnly;
                                    btnShowTrades.Background = Brushes.DimGray;
                                    break;
                                }
                            case ChartExecutionStyle.MarkersOnly:
                                {
                                    trades.Properties.PlotExecutions = ChartExecutionStyle.DoNotPlot;
                                    btnShowTrades.Background = Brushes.Green;
                                    break;
                                }
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
        /// <summary>
        /// Create 'C:\data\csvNTDrawline.csv'
        /// </summary>
        /// 
        private void ReadCsvAndDrawLines()
        {
            #region Use LINQtoCSV to read "csvNTDrawline.csv"
            CsvFileDescription scvDescript = new CsvFileDescription();
            CsvContext cc = new CsvContext();
            CsvFileDescription dataFromFile = new CsvFileDescription();

            //	Read in file 'C:\data\csvNTDrawline.csv'  Fills returnedClass
            //	Conflict with Class NTDrawLine - in both getInstListFutures and getInstListFuturesExtension 
            //IEnumerable<NinjaTrader.Custom.AddOns.NTDrawLine> returnedClass =
            //    cc.Read<NinjaTrader.Custom.AddOns.NTDrawLine>

            returnedClass = cc.Read<NinjaTrader.Custom.AddOns.NTDrawLine>
                            (
                                //InputFile,
                                OutputFile,
                            //@"C:\data\csvNTDrawline.csv",
                            //@"C:\Users\Owner\Documents\NinjaTrader 8\csvNTDrawline.csv",
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

                i++;
            }
            #endregion foreach() Through returnedClass and Draw line


        }
        private void CreateCvsFunc()
        {
            var bPlayback = false;
            //  get first and last bar on chart
            //   use these when giving the .csv file a name in 
            /// 'SqLiteExecutionsToListAndQueryResults.Program.main(parameters);'
            inputFirstBarTime = ChartBars.GetTimeByBarIdx(ChartControl, ChartBars.FromIndex);
            inputFirstBarOnChart = inputFirstBarTime.ToString("yy MM dd HH_mm");
            inputLastBarTime = ChartBars.GetTimeByBarIdx(ChartControl, ChartBars.ToIndex);
            inPutLastBarOnChart = inputLastBarTime.ToString("yy MM dd HH_mm");

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

            List<CSV.CSV> CSv = new List<CSV.CSV>();
            //  list to hold valiables in Executions table from NinjaTrader.sqlite
            List<ExecutionsClass.Executions> listExecution = new List<ExecutionsClass.Executions>();
            List<NTDrawLine.NTDrawLine> nTDrawline = new List<NTDrawLine.NTDrawLine>();
            //  list to hold Ret() format from listExecution
            List<Ret.Ret> listExecutionRet = new List<Ret.Ret>();
            Source.Source source = new Source.Source();
            //List<Query> selectedList = new List<Query>();
            List<Trade.Trade> workingTrades = new List<Trade.Trade>();
            List<Trade.Trade> trades = new List<Trade.Trade>();
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
			return CacheIndicator<My.RecordAndDisplayTradesWithButtonsRyzen2>(new My.RecordAndDisplayTradesWithButtonsRyzen2(){ StartTime = startTime, EndTime = endTime, InputFile = inputFile, OutputFile = outputFile, colorActiveCursor = colorActiveCursor }, input, ref cacheRecordAndDisplayTradesWithButtonsRyzen2);
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

		public Indicators.My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(ISeries<double> input , DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor)
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

		public Indicators.My.RecordAndDisplayTradesWithButtonsRyzen2 RecordAndDisplayTradesWithButtonsRyzen2(ISeries<double> input , DateTime startTime, DateTime endTime, string inputFile, string outputFile, bool colorActiveCursor)
		{
			return indicator.RecordAndDisplayTradesWithButtonsRyzen2(input, startTime, endTime, inputFile, outputFile, colorActiveCursor);
		}
	}
}

#endregion
