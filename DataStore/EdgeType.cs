using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public class EdgeType
    {
        public string Name { get; private set; }
        public string ReverseName { get; private set; }

        public EdgeType(string name, string reverseName)
        {
            Name = name;
            ReverseName = reverseName;
        }
    }
}
