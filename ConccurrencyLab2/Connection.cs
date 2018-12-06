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

            //Message: Server doesn't know the client number -> double connection
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
                    //AANPASSEN: wat je binnen krijgt, kijken wat het is en dan reageren daarop. (soort bericht: bv verbreek verbinding)
                    Console.WriteLine(Read.ReadLine());
            }
            catch { }
        }
    }
}
