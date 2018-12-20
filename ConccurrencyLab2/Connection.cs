using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConccurrencyLab2
{
    class Connection
    {
        public StreamReader Read;
        public StreamWriter Write;
        public int PortNr;

        //Client worden bij een andere server
        public Connection(int port)
        {
            TcpClient client = new TcpClient("localhost", port);
            PortNr = port;
            Read = new StreamReader(client.GetStream());
            Write = new StreamWriter(client.GetStream());
            //direct sending of message through AutoFlush
            Write.AutoFlush = true;

            //Message: Client doesn't know the Server number -> double connection
            Write.WriteLine("Port: " + Program.MyPortNr);

            //Constante state of reading 
            new Thread(ThreadReader).Start();
        }


        //Server bent en een client vraag verbinding aan
        public Connection(int port, StreamReader read, StreamWriter write)
        {
            PortNr = port;
            Read = read;
            Write = write;

            //Start constant state of reading
            new Thread(ThreadReader).Start();
        }

        public void ThreadReader()
        {
            try
            {
                while (true)
                {
                    string input = Read.ReadLine();
                    string[] inputArr = input.Split();

                    switch (inputArr[0])
                    {
                        case "U":
                            {
                                UpdateRoutingTable(inputArr);
                            }
                            break;
                        case "B":
                            {
                                if (int.Parse(inputArr[1]) == Program.MyPortNr)
                                {
                                    //Write message in console
                                    string[] message = input.Split(new char[] { ' ' }, 3);
                                    Console.WriteLine(message[2]);
                                }
                                else
                                {
                                    //bericht is niet voor deze console, stuur hem verder

                                    Program.SendMessage(input);
                                }
                            }
                            break;
                    }
                }
            }
            catch { }
        }

        public void UpdateRoutingTable(string[] input)
        {
            lock (Program._Lock)
            {
                int portNr = int.Parse(input[1]);
                int dist = int.Parse(input[2]) + 1;
                bool inRoutingTable = false;

                //Check if the node is already in the routingtable
                foreach (Node node in Program.routingTable)
                {
                    if (node.portNr == portNr)
                    {
                        if (dist < node.dist)
                        {
                            node.otherRoute.Add(node.lastNode, node.dist);
                            node.dist = dist;
                            node.lastNode = PortNr;
                            Console.WriteLine("Afstand naar {0} is nu {1} via {2}", node.portNr, node.dist, node.lastNode);
                            Program.SendRoutingTable();
                        }
                        else
                        {
                            if (PortNr != node.lastNode && portNr != Program.MyPortNr)
                            {
                                //check if there is already an other route via this node, if so if this route is shorter change de dist
                                if (!node.otherRoute.ContainsKey(PortNr))
                                    node.otherRoute.Add(PortNr, dist);
                                else
                                {
                                    if (node.otherRoute[PortNr] > dist)
                                        node.otherRoute[PortNr] = dist;
                                }
                            }
                        }
                        inRoutingTable = true;
                    }
                }
                //if its not in the routingtable
                if (!inRoutingTable)
                {
                    Node newNode = new Node(portNr, dist, PortNr, new Dictionary<int, int>());
                    Program.routingTable.Add(newNode);
                    Console.WriteLine("Afstand naar {0} is nu {1} via {2}", newNode.portNr, newNode.dist, newNode.lastNode);
                    Program.SendRoutingTable();
                }
            }
        }
    }
}
