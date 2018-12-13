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
        public static List<Node> routingTable = new List<Node>();
        public static Dictionary<int, Connection> Neighbours = new Dictionary<int, Connection>();
        public static object _Lock = new object();

        static void Main(string[] args)
        {
            //get portnumber and set console name to the portnumber
            MyPortNr = int.Parse(args[0]);
            Console.Title = "NetChange " + MyPortNr;
            new Server(MyPortNr);
            routingTable.Add(new Node(MyPortNr, 0, MyPortNr, new Dictionary<int, int>()));
                                    
            int NumbNeighbours = args.Length - 1;
            for (int i = 0; i < NumbNeighbours; i++)
            {
                int PortNeighbour = int.Parse(args[i + 1]);
                if (PortNeighbour > MyPortNr) //connect only with higher portnumbers
                {
                    lock (_Lock)
                    {
                        //set connections
                        Neighbours.Add(PortNeighbour, new Connection(PortNeighbour));
                    }
                }
            }

            PrintRoutingTable();

            SendRoutingTable();

            HandleInput();
        }

        public static void SendRoutingTable()
        {
            foreach (int Neighbour in Neighbours.Keys) //voor alle connecties
            {
                foreach (Node N in routingTable)
                {
                    Neighbours[Neighbour].Write.WriteLine("U " + N.getUpdateString());
                }
            }
        }

        static void HandleInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                switch (input.Split()[0])
                {
                    case "T":
                        PrintRoutingTable();
                        break;
                }
            }
        }

        public static void PrintRoutingTable()
        {
            Console.WriteLine("-");
            lock (_Lock)
            {
                foreach (Node N in routingTable)
                {
                    if (N.dist == 0)
                        Console.WriteLine(N.portNr + " " + N.dist + " local");
                    else
                        Console.WriteLine(N.portNr + " " + N.dist + " " + N.lastNode);
                }
            }
        }
    }
}