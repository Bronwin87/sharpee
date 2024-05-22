using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public abstract class GraphEvent
    {
        // Common properties and methods for graph events
    }

    public class NodeAddedEvent : GraphEvent
    {
        public Node AddedNode { get; }

        public NodeAddedEvent(Node node)
        {
            AddedNode = node;
        }
    }

    public class NodeRemovedEvent : GraphEvent
    {
        public Node RemovedNode { get; }

        public NodeRemovedEvent(Node node)
        {
            RemovedNode = node;
        }
    }

    public class EdgeAddedEvent : GraphEvent
    {
        public Edge AddedEdge { get; }

        public EdgeAddedEvent(Edge edge)
        {
            AddedEdge = edge;
        }
    }

    public class EdgeRemovedEvent : GraphEvent
    {
        public Edge RemovedEdge { get; }

        public EdgeRemovedEvent(Edge edge)
        {
            RemovedEdge = edge;
        }
    }

    public class PropertyChangedEvent : GraphEvent
    {
        public string NodeOrEdgeId { get; }
        public Property Property { get; }

        public PropertyChangedEvent(string nodeOrEdgeId, Property property)
        {
            NodeOrEdgeId = nodeOrEdgeId;
            Property = property;
        }
    }

    // ... existing classes (Node, Edge, Property, World, etc.) ...
}
