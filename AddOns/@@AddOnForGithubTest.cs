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
using NinjaTrader.NinjaScript.Indicators.My;
using NinjaTrader.NinjaScript;


namespace NinjaTrader.Custom.AddOns
{
    public class PrintMessage
    {
       	
        public string message = "msg";
        public PrintMessage()
        {
            Console.WriteLine(message);
        }

        public void Printer()
        {
            //Set this scripts Print() calls to the first output tab
//            PrintTo p = PrintTo.OutputTab1;
//            p.
//            Console.WriteLine("");
        }
        public double Offset { get; private set; }
    }
}