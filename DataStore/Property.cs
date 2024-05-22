using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public class Property
    {
        public string Name { get; private set; }
        public object Value { get; set; }

        public Property(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
