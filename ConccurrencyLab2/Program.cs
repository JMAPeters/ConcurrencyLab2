using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConccurrencyLab2
{
    class Program
    {
        static int MyPortNr;

        static void Main(string[] args)
        {
            MyPortNr = int.Parse(args[0]);
            Console.Title = "NetChange " + MyPortNr;
            Console.ReadLine();
        }
    }
}
