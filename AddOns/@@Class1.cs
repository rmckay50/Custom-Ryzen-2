using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators.My;

namespace NinjaTrader.Custom.AddOns
{
    public class Class1
    {
		public Class1(){}
		public void dump()
		{
            // Instead of Print()
            NinjaTrader.Code.Output.Process("my message", PrintTo.OutputTab1);
        }
    }
}
