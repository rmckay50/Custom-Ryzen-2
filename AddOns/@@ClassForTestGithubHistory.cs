using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NinjaTrader.NinjaScript;

namespace NinjaTrader.Custom.AddOns
{
    public class ClassForTestGithubHistory
    {
		public void Display()
		{
//			NinjaTrader.NinjaScript.ClearOutputWindow();
			NinjaTrader.Code.Output.Process("my message", PrintTo.OutputTab1);
		}
    }
}
