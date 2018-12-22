using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Programma runt correct wanneer het zelf gedraaid word vanuit de system.bat
 * sturen van routingtable werkt,
 * bericht sturen werkt,
 * niewe connectie maken werkt,
 * disconnect werkt behalve,
 * Netwerkt partities is niet correct gelukt.
 * Het is alleen onstabiel wanneer het in tomjudge word gedraait, de ene keer runt die wel goed de andere keer niet
 * Dit ligt waarschijnlijk aan de locks. 
 * Er is heel veel getest met het zetten van locks echter resulteerde dit nooit in een stabiele build. 
 * Deze versie is de meest stabiele build
 */


namespace ConccurrencyLab2
{
    class Program
    {
        public static int MyPortNr;
        public static List<Node> routingTable = new List<Node>();
        public static Dictionary<int, Connection> Neighbours = new Dictionary<int, Connection>();
        public static object _Lock = new object();
        public static object _LockTable = new object();

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
                    lock (_LockTable)
                    {
                        foreach (Node N in routingTable)
                        {
                            Neighbours[Neighbour].Write.WriteLine("U " + N.getUpdateString());
                        }
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
                    case "D":
                        Disconnect(input);
                        break;
                }
            }
        }

        public static void SendMessage(string input)
        {
            //lock (_LockTable)
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
        }

        public static void NewConnection(string input)
        {
            lock (_Lock)
            {
                int portNr = int.Parse(input.Split()[1]);

                if (!Neighbours.ContainsKey(portNr))
                {
                    //set connections
                    Neighbours.Add(portNr, new Connection(portNr)); //DIT WERKT NOG NIET
                    Console.WriteLine("Verbonden: {0}", portNr);
                }
                else
                    Console.WriteLine("Poort {0} is niet bekend", portNr);
            }
            SendRoutingTable();
        }

        public static void Disconnect(string input)
        {
            lock (_Lock)
            {
                int PortNumber = int.Parse(input.Split()[1]);
                //Je wil eerst checken of je een connectie hebt met de betreffende node.
                if (Neighbours.ContainsKey(PortNumber))
                {
                    ////eerst bericht naar alle buren dat we iemand er uit gaan FLIKKEREN
                    foreach (int Neighbour in Neighbours.Keys)
                    {
                        Neighbours[Neighbour].Write.WriteLine(input);
                    }
                    //Flikker de node uit je Neigbourslist
                    Neighbours.Remove(PortNumber);
                    Console.WriteLine("Verbroken: {0}", PortNumber);
                }
                else
                    Console.WriteLine("Poort {0} is niet bekend", PortNumber);
            }

            lock (_LockTable)
            {
                int PortNumber = int.Parse(input.Split()[1]);
                if (!Neighbours.ContainsKey(PortNumber))
                {
                    //Als ie niet in je neigbourslist zit wil je gaan herberekenen.
                    //stuur naar je eigen neigbours dat papi node de aarde heeft verlaten.

                    // als die er nog in zit -> nog maken
                    //dan verwijderen en bericht doorsturen
                    foreach (int Neighbour in Neighbours.Keys)
                    {
                        Neighbours[Neighbour].Write.WriteLine(input);
                    }
                    //Ook verwijderen uit je routingTable, kijk voor elke node.portnumber of is gelijk is met je te verwijderen port number
                    //lock (_LockTable)
                    {
                        foreach (Node node in routingTable)
                        {
                            if (node.portNr == PortNumber)
                            {

                                routingTable.Remove(node);
                            }

                            //om verwijderde node uit otherroutes in routingtable te verwijderen
                            if (node.otherRoute.ContainsKey(PortNumber))
                            {
                                Console.WriteLine("verwijderd uit otherroutes");
                                node.otherRoute.Remove(PortNumber);
                            }

                            if (node.lastNode == PortNumber)
                            {
                                int dist = 21;
                                int lastNode = 0;
                                foreach (KeyValuePair<int, int> route in node.otherRoute)
                                {
                                    if (route.Value < dist)
                                    {
                                        dist = route.Value;
                                        lastNode = route.Key;
                                    }
                                }
                                Console.WriteLine("{0} {1}", dist, lastNode);
                                node.dist = dist;
                                node.lastNode = lastNode;
                            }
                        }
                    }
                }
            }
        }

        public static void PrintRoutingTable()
        {
            lock (_LockTable)
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