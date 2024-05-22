using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public class Node
    {
        public string Id { get; private set; }
        public object Data { get; private set; }
        public List<Edge> Edges { get; private set; }
        public List<Property> Properties { get; private set; }

        public Node(string id, object data)
        {
            Id = id;
            Data = data;
            Edges = new List<Edge>();
            Properties = new List<Property>();
        }
    }
}
