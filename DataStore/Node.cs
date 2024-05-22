using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public class Node
    {
        public string Id { get; }
        public List<Edge> Edges { get; }
        public List<Property> Properties { get; }

        public Node(string id)
        {
            Id = id;
            Edges = new List<Edge>();
            Properties = new List<Property>();
        }

        public Node(string id, List<Property> properties)
            : this(id)
        {
            if (properties != null)
            {
                Properties.AddRange(properties);
            }
        }

        public T GetPropertyValue<T>(string propertyName)
        {
            var property = Properties.FirstOrDefault(p => p.Name == propertyName);
            return property != null ? (T)property.Value : default(T);
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            var property = Properties.FirstOrDefault(p => p.Name == propertyName);
            if (property != null)
            {
                property.Value = value;
            }
            else
            {
                Properties.Add(new Property(propertyName, value));
            }
        }
    }
}