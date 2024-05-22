using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public class World
    {
        public Dictionary<string, Node> Nodes { get; }
        public Dictionary<string, EdgeType> EdgeTypes { get; }

        private List<IGraphEventHandler> eventHandlers = new List<IGraphEventHandler>();

        public World()
        {
            Nodes = new Dictionary<string, Node>();
            EdgeTypes = new Dictionary<string, EdgeType>();
        }

        public void AddEdgeType(string name, string reverseName)
        {
            EdgeTypes[name] = new EdgeType(name, reverseName);
        }

        public EdgeType GetEdgeType(string name)
        {
            return EdgeTypes[name];
        }

        public string AddNode(params Property[] properties)
        {
            bool checkNameExists = properties.ToList<Property>().Exists(p => p.Name == "name");
            bool checkTypeExists = properties.ToList<Property>().Exists(p => p.Name == "type");

            if (!checkNameExists || !checkTypeExists)
                throw new Exception("New nodes require name and type properties.");

            string id = GenerateUniqueNodeId();

            var node = new Node(id, properties.ToList<Property>());
            Nodes[id] = node;
            PublishNodeAdded(node);
            return id;
        }

        public void RemoveNode(string id)
        {
            if (Nodes.ContainsKey(id))
            {
                var node = Nodes[id];
                Nodes.Remove(id);
                PublishNodeRemoved(node);
            }
        }

        public void ConnectNodes(string id1, string id2, string edgeType, string reverseEdgeType, params Property[]? properties)
        {
            List<Property> propertyList = new List<Property>();
            if (properties != null)
            {
                propertyList = properties.ToList<Property>();
            }

            Edge edge1 = new Edge(id1, id2, edgeType, propertyList);
            Nodes[id1].Edges.Add(edge1);
            PublishEdgeAdded(edge1);

            if (reverseEdgeType != null)
            {
                Edge edge2 = new Edge(id2, id1, reverseEdgeType, propertyList);
                Nodes[id2].Edges.Add(edge2);
                PublishEdgeAdded(edge2);
            }
        }

        public void DisconnectNodes(string id1, string id2)
        {
            var edge1 = Nodes[id1].Edges.Find(e => e.Id2 == id2);
            if (edge1 != null)
            {
                Nodes[id1].Edges.Remove(edge1);
                PublishEdgeRemoved(edge1);
            }

            var edge2 = Nodes[id2].Edges.Find(e => e.Id1 == id2);
            if (edge2 != null)
            {
                Nodes[id2].Edges.Remove(edge2);
                PublishEdgeRemoved(edge2);
            }
        }

        public void SetNodeProperty(string nodeId, string propertyName, object propertyValue)
        {
            var property = Nodes[nodeId].Properties.Find(p => p.Name == propertyName);
            if (property != null)
            {
                property.Value = propertyValue;
            }
            else
            {
                property = new Property(propertyName, propertyValue);
                Nodes[nodeId].Properties.Add(property);
            }
            PublishPropertyChanged(nodeId, property);
        }

        public void SetEdgeProperty(string id1, string id2, string propertyName, object propertyValue)
        {
            var edge = Nodes[id1].Edges.Find(e => e.Id2 == id2);
            if (edge != null)
            {
                var property = edge.Properties.Find(p => p.Name == propertyName);
                if (property != null)
                {
                    property.Value = propertyValue;
                }
                else
                {
                    property = new Property(propertyName, propertyValue);
                    edge.Properties.Add(property);
                }
                PublishPropertyChanged($"{id1}-{id2}", property);
            }
        }

        public void AddEventHandler(IGraphEventHandler handler)
        {
            eventHandlers.Add(handler);
        }

        public void RemoveEventHandler(IGraphEventHandler handler)
        {
            eventHandlers.Remove(handler);
        }

        private void PublishNodeAdded(Node node)
        {
            foreach (var handler in eventHandlers)
            {
                handler.HandleNodeAdded(node);
            }
        }

        private void PublishNodeRemoved(Node node)
        {
            foreach (var handler in eventHandlers)
            {
                handler.HandleNodeRemoved(node);
            }
        }

        private void PublishEdgeAdded(Edge edge)
        {
            foreach (var handler in eventHandlers)
            {
                handler.HandleEdgeAdded(edge);
            }
        }

        private void PublishEdgeRemoved(Edge edge)
        {
            foreach (var handler in eventHandlers)
            {
                handler.HandleEdgeRemoved(edge);
            }
        }

        private void PublishPropertyChanged(string nodeOrEdgeId, Property property)
        {
            foreach (var handler in eventHandlers)
            {
                handler.HandlePropertyChanged(nodeOrEdgeId, property);
            }
        }

        private string GenerateUniqueNodeId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

            string id;
            do
            {
                id = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
            } while (Nodes.ContainsKey(id));

            return id;
        }
    }
}