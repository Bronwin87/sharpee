using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public class Behavior
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public Behavior(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }

    public class Scenery : Behavior
    {
        public Scenery(string name, object value) : base(name, value)
        {
        }
    }

    public class  Container: Behavior
    {
        public Container(string name, object value) : base(name, value)
        {
        }
    }
}
