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
                    Console.WriteLine("Verbonden: {0}", PortNeighbour);
                }
            }

            SendRoutingTable();

            HandleInput();
        }

        public static void SendRoutingTable()
        {
            lock (_Lock)
            {
                foreach (int Neighbour in Neighbours.Keys) //voor alle connecties
                {
                    foreach (Node N in routingTable)
                    {
                        Neighbours[Neighbour].Write.WriteLine("U " + N.getUpdateString());
                    }
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
                    case "R":
                        PrintRoutingTable();
                        break;
                    case "B":
                        SendMessage(input);
                        break;
                    case "C":
                        NewConnection(input);
                        break;
                }
            }
        }

        public static void SendMessage(string input)
        {
            bool isFound = false;
            int Portnumber = int.Parse(input.Split()[1]);

            foreach (Node N in routingTable)
            {
                //als waar, bestaat er dus een portnumber waar je je bericht heen kan sturen
                if (N.portNr == Portnumber)
                {
                    //In de neighbors lijst wil je zoeken naar de juiste neighbour, en die moet het weer doorsturen naar de betreffende node
                    Neighbours[N.lastNode].Write.WriteLine(input);
                    isFound = true;
                    Console.WriteLine("Bericht voor {0} doorgestuurd naar {1}", Portnumber, N.lastNode);
                }
            }
            if (!isFound)
                Console.WriteLine("Poort {0} is niet bekend", Portnumber);
        }

        public static void NewConnection(string input)
        {
            int portNr = int.Parse(input.Split()[1]);
            bool isNeighbour = false;

            foreach (int Neighbour in Neighbours.Keys)
            {
                if (portNr == Neighbour)
                {
                    isNeighbour = true;
                }

            }

            if (!isNeighbour)
            {
                lock (_Lock)
                {
                    //set connections
                    //Neighbours.Add(portNr, new Connection(portNr)); DIT WERKT NOG NIET
                }
                Console.WriteLine("Verbonden: {0}", portNr);
                //SendRoutingTable();
            }
            else
                Console.WriteLine("Poort {0} bestaat al", portNr);
        }

        public static void PrintRoutingTable()
        {
            lock (_Lock)
            {
                foreach (Node N in routingTable)
                {
                    if (N.dist == 0)
                        Console.WriteLine(N.portNr + " " + N.dist + " local");
                    else
                    {
                        string info = N.portNr + " " + N.dist + " " + N.lastNode + " other route: ";
                        foreach (KeyValuePair<int, int> route in N.otherRoute)
                            info += route.Key + " " + route.Value + " ";
                        Console.WriteLine(info);
                    }
                }
            }
        }
    }
}