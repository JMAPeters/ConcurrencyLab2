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
        static int[] Neighbours; //array with own neightbours

        static void Main(string[] args)
        {
            //get portnumber and set console name to the portnumber
            MyPortNr = int.Parse(args[0]);
            Console.Title = "NetChange " + MyPortNr;

            //set connections
            Neighbours = new int[args.Length - 1];
            for (int i = 1; i <= args.Length - 1; i++)
            {
                Neighbours[i] = int.Parse(args[i]);
            }


            //TODO: maak een dictunariy met 2 ints, waar je naar toe kan en wat daar de vorige node van was.
            //zorg dat de console niet afsluit
            Console.ReadLine();
        }
    }
}
