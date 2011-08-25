using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GistForVS.Tests.TestHelpers
{
    public class WpfHelper
    {
        public static void RunBlockAsSTA(Action block)
        {
            Exception ex = null;
            var t = new Thread(() =>
            {
                try
                {
                    block();
                }
                catch(Exception e)
                {
                    ex = e;
                    throw;
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            if (ex != null)
            {
                // NB: If we don't do this, the test silently passes
                throw ex;
            }
        }
    }
}
