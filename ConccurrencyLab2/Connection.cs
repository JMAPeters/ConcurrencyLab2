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

        //Client worden bij een andere server
        public Connection(int port)
        {
            TcpClient client = new TcpClient("localhost", port);
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
        public Connection(StreamReader read, StreamWriter write)
        {
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
                    Console.WriteLine(Read.ReadLine());
                    //AANPASSEN: wat je binnen krijgt, kijken wat het is en dan reageren daarop. (soort bericht: bv verbreek verbinding)

                    //string[] input = Read.ReadLine().Split();
                    //switch (input[0])
                    //{
                    //    case "U":
                    //        {
                    //            //UpdateRoutingTable(input);
                    //        }
                    //        break;
                    //}
                }
                    
            }
            catch { }
        }

        public void UpdateRoutingTable(string[] input)
        {
            bool inRoutingTable = false;
            foreach (Node N in Program.routingTable)
            {
                if (N.portNr == int.Parse(input[1]))
                    inRoutingTable = true;
            }
            if (!inRoutingTable)
            {
                Node newNode = new Node(int.Parse(input[1]), int.Parse(input[2]), int.Parse(input[3]), null);
                for (int i = 0; i <= int.Parse(input[4]); i++)
                {
                    newNode.otherRoute.Add(int.Parse(input[4 + (i * 2)]), int.Parse(input[4 + (i * 2) + 1]));
                }
                Program.routingTable.Add(newNode);
            }
        }
    }
}
