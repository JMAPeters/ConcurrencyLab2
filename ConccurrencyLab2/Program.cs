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
            routingTable.Add(new Node(MyPortNr, 0, MyPortNr, null));
                                    
            int NumbNeighbors = args.Length - 1;
            for (int i = 0; i < NumbNeighbors; i++)
            {
                int PortNeighbour = int.Parse(args[i + 1]);
                lock (_Lock)
                {
                    //set connections
                    Neighbours.Add(PortNeighbour, new Connection(PortNeighbour));
                }
            }

            PrintRoutingTable();

            SendRoutingTable();

            PrintRoutingTable();

            //HandleInput();
        }

        static void SendRoutingTable()
        {
            if (Neighbours.ContainsKey(1101))
            {
                Console.WriteLine("test4321");
                lock (_Lock)
                {
                    Neighbours[1101].Write.WriteLine("test1234");
                }
            }
            //foreach (int Neighbour in Neighbours.Keys) //voor alle connecties
            //
            //    foreach (Node N in routingTable)
            //    {
            //        Neighbours[Neighbour].Write.WriteLine("U " + N.getUpdateString());
            //        Console.WriteLine(Neighbour + " U " + N.getUpdateString());
            //    }
            //}
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

        static void PrintRoutingTable()
        {
            foreach (Node N in routingTable)
            {
                Console.WriteLine(N.portNr + " " + N.dist + " " + N.lastNode);
            }
        }
    }
}