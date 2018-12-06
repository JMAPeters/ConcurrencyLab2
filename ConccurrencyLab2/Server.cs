using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConccurrencyLab2
{
    class Server
    {
        public Server(int port)
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            new Thread(() => AcceptLoop(server)).Start();
        }

        private void AcceptLoop(TcpListener _server)
        {
            while (true)
            {
                TcpClient client = _server.AcceptTcpClient();
                StreamReader clientIn = new StreamReader(client.GetStream());
                StreamWriter clientOut = new StreamWriter(client.GetStream());

                clientOut.AutoFlush = true;

                int clientPort = int.Parse(clientIn.ReadLine().Split()[1]); //krijgt van connection clientPort binnen.

                Program.routingTable.Add(clientPort, new Connection(clientIn, clientOut));
            }
        }
    }
}
