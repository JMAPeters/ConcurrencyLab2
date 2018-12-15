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
                    //AANPASSEN: wat je binnen krijgt, kijken wat het is en dan reageren daarop. (soort bericht: bv verbreek verbinding)

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
                                    Console.WriteLine("Message received:" + input);
                                //bericht is niet voor deze console, stuur hem verder
                                else
                                {
                                    Console.WriteLine("Test");
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
            bool inRoutingTable = false;
            int port = int.Parse(input[1]);
            int dist = int.Parse(input[2]) + 1;
            int lastNode = int.Parse(input[3]);

            foreach (Node N in Program.routingTable)
            {
                if (N.portNr == port)
                {
                    if (N.dist < dist)
                    {
                        N.otherRoute.Add(dist, PortNr);
                        Console.WriteLine(dist + " " + PortNr);
                    }
                    else
                    {
                        N.otherRoute.Add(N.dist, N.lastNode);
                        N.dist = dist;
                        N.lastNode = PortNr;
                    }

                    inRoutingTable = true;
                }
            }
            if (!inRoutingTable)
            {
                Node newNode = new Node(port, dist + 1, PortNr, new Dictionary<int, int>()); //client port hops lastNode
                //for (int i = 0; i < int.Parse(input[4]); i++)
                //{
                //    newNode.otherRoute.Add(int.Parse(input[4 + (i * 2)]), int.Parse(input[4 + (i * 2) + 1]));
                //}
                lock (Program._Lock)
                    Program.routingTable.Add(newNode);

                Program.SendRoutingTable();
            }
        }
    }
}
