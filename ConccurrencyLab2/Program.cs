using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConccurrencyLab2
{
    class Program
    {
        public static int MyPortNr;
        static int[] Neighbours; //array with own neightbours Nbu[v] forwarding table
        public static Dictionary<int, Connection> routingTable = new Dictionary<int, Connection>();

        static void Main(string[] args)
        {
            //get portnumber and set console name to the portnumber
            MyPortNr = int.Parse(args[0]);
            Console.Title = "NetChange " + MyPortNr;
            new Server(MyPortNr);

            //set connections
            int NumbNeighbours = args.Length - 1;
            Neighbours = new int[NumbNeighbours];//////////////////////////////////////////Is neighbour array wel nodig??????
            for (int i = 0; i < NumbNeighbours; i++)
            {
                int PortNeighbour = int.Parse(args[i + 1]);
                Neighbours[i] = PortNeighbour;
                //Console.WriteLine(Neighbours[i]);
                routingTable.Add(PortNeighbour, new Connection(PortNeighbour));
            }

            HandleInput();
        }

        static void HandleInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                switch (input.Split()[0])
                {
                    case "R":
                        Console.WriteLine("verstuur sting naar poort");
                        break;
                }
            }
        }

        static void UpdateRoutingTable()
        {

        }
    }
}

/*
 * Geef buren meteen door wanneer je je eigen poortnummer door geeft
 * update routing tabel. bij routingTable[poort].Update -> update geeft zijn neighbours lijst terug -> dus wanneer verwijderen of toevoegen bij neighbours veranderen
 * updaten voordat je iets doet
 */