using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace BtKeySync
{
    class Program
    {
        static int Main(string[] args)
        {
            BtKeySync.Run(new BtKeySync());

            return 0;
        }
    }
}
