using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NinjaTrader.Code;
using NinjaTrader.NinjaScript;

namespace NinjaTrader.Custom.AddOns
{
    public class ClassForTestGithubHistory
    {
		public void Display()
        {
//            NinjaTrader.Code.Output.Reset(PrintTo.OutputTab1);
            NinjaTrader.Code.Output.Process("my message", PrintTo.OutputTab1);
		}
    }
}
