using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConccurrencyLab2
{
    class Node
    {
        public int portNr;
        public int dist;
        public int lastNode;
        public Dictionary<int, int> otherRoute;

        public string UpdateString;

        public Node(int _portNr, int _dist, int _lastNode, Dictionary<int, int> _otherRoute)
        {
            portNr = _portNr;
            dist = _dist;
            lastNode = _lastNode;
            otherRoute = _otherRoute;
            
        }

        public string getUpdateString()
        {
            UpdateString = portNr + " " + dist;
            return UpdateString;
        }
    }
}
