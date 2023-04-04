#region Comments
/*  2022 11 18 1525
 *      Add to NinjaScript to log errors
 *      
 *      Include:
 *          using NinjaTrader.Custom.AddOns;
 *          try
 *          {}
 *          catch(Exception ex)
 *          {
 *              TryCatchClass.TryCatch(ex);
 *          }      
 *      Exception details are printed to C:\Error.txt
 */
#endregion Comments

#region Using declarations
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
#endregion
namespace NinjaTrader.Custom.AddOns
{
    public static class TryCatchClass
    {
        public static void TryCatch(Exception ex)
        {

            string filePath = @"C:Error\Error.txt";
            int linenumber = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();
                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);
                    

                    ex = ex.InnerException;
                }
            }
        }
    }
}
