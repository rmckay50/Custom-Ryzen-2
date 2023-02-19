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
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.Gui.Tools;
#endregion

//This namespace holds Add ons in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.AddOns
{
	
	
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


		public ReturnedClass() { }

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

#endregion



}
