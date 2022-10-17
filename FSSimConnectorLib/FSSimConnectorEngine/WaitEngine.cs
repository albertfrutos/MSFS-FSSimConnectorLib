using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FSSimConnectorLib
{
    internal class WaitEngine
    {
        internal int WaitMs { get; set; }

        internal void WaitMilliseconds(int millis)
        {
            Console.WriteLine("start wait");
            Thread.Sleep(millis);
            Console.WriteLine("finish wait");
        }
    }
}
