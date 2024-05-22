using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public class Edge
    {
        public string Id1 { get; private set; }
        public string Id2 { get; private set; }
        public string EdgeType { get; private set; }
        public List<Property> Properties { get; private set; }

        public Edge(string id1, string id2, string edgeType)
        {
            Id1 = id1;
            Id2 = id2;
            EdgeType = edgeType;
            Properties = new List<Property>();
        }
    }

}
