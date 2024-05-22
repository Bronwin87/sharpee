using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DataStore;

namespace WorldModel
{
    public class TraversalService
    {
        private readonly World _world;

        public TraversalService(World world)
        {
            _world = world;
        }

        public List<Node>? FindPath(string startNodeId, string endNodeId)
        {
            var startNode = _world.Nodes[startNodeId];
            var endNode = _world.Nodes[endNodeId];

            var visitedNodes = new HashSet<string>();
            var path = new List<Node>();

            if (FindPathRecursive(startNode, endNode, visitedNodes, path))
            {
                return path;
            }

            return null;
        }

        private bool FindPathRecursive(Node currentNode, Node endNode, HashSet<string> visitedNodes, List<Node> path)
        {
            if (currentNode == endNode)
            {
                path.Add(currentNode);
                return true;
            }

            visitedNodes.Add(currentNode.Id);
            path.Add(currentNode);

            foreach (var edge in currentNode.Edges)
            {
                var nextNodeId = edge.Id1 == currentNode.Id ? edge.Id2 : edge.Id1;
                if (!visitedNodes.Contains(nextNodeId))
                {
                    var nextNode = _world.Nodes[nextNodeId];
                    if (FindPathRecursive(nextNode, endNode, visitedNodes, path))
                    {
                        return true;
                    }
                }
            }

            path.RemoveAt(path.Count - 1);
            return false;
        }

        public List<Node> GetConnectedNodes(string nodeId)
        {
            var node = _world.Nodes[nodeId];
            var connectedNodes = new List<Node>();

            foreach (var edge in node.Edges)
            {
                var connectedNodeId = edge.Id1 == nodeId ? edge.Id2 : edge.Id1;
                var connectedNode = _world.Nodes[connectedNodeId];
                connectedNodes.Add(connectedNode);
            }

            return connectedNodes;
        }

        public List<Node> GetAdjacentNodes(string nodeId, string edgeType)
        {
            var node = _world.Nodes[nodeId];
            var adjacentNodes = new List<Node>();

            foreach (var edge in node.Edges)
            {
                if (edge.EdgeType == edgeType)
                {
                    var adjacentNodeId = edge.Id1 == nodeId ? edge.Id2 : edge.Id1;
                    var adjacentNode = _world.Nodes[adjacentNodeId];
                    adjacentNodes.Add(adjacentNode);
                }
            }

            return adjacentNodes;
        }
        public List<Node> FindNodesWithProperty(string propertyName, object propertyValue)
        {
            return _world.Nodes.Values
                .Where(node => node.Properties.Any(prop => prop.Name == propertyName && prop.Value.Equals(propertyValue)))
            .ToList();
        }

        public List<Node> FindNodesWithProperties(Dictionary<string, object> properties)
        {
            return _world.Nodes.Values
                .Where(node => properties.All(prop => node.Properties.Any(p => p.Name == prop.Key && p.Value.Equals(prop.Value))))
                .ToList();
        }

        public List<Edge> FindEdgesWithProperty(string propertyName, object propertyValue)
        {
            return _world.Nodes.Values
                .SelectMany(node => node.Edges)
                .Where(edge => edge.Properties.Any(prop => prop.Name == propertyName && prop.Value.Equals(propertyValue)))
                .ToList();
        }

        public List<Edge> FindEdgesWithProperties(Dictionary<string, object> properties)
        {
            return _world.Nodes.Values
                .SelectMany(node => node.Edges)
                .Where(edge => properties.All(prop => edge.Properties.Any(p => p.Name == prop.Key && p.Value.Equals(prop.Value))))
                .ToList();
        }

        public List<Node> FindConnectedNodesWithProperty(string nodeId, string propertyName, object propertyValue)
        {
            var node = _world.Nodes[nodeId];
            return node.Edges
                .Select(edge => edge.Id1 == nodeId ? _world.Nodes[edge.Id2] : _world.Nodes[edge.Id1])
                .Where(connectedNode => connectedNode.Properties.Any(prop => prop.Name == propertyName && prop.Value.Equals(propertyValue)))
                .ToList();
        }

        public List<Node> FindAdjacentNodesWithProperty(string nodeId, string edgeType, string propertyName, object propertyValue)
        {
            var node = _world.Nodes[nodeId];
            return node.Edges
                .Where(edge => edge.EdgeType == edgeType)
                .Select(edge => edge.Id1 == nodeId ? _world.Nodes[edge.Id2] : _world.Nodes[edge.Id1])
                .Where(adjacentNode => adjacentNode.Properties.Any(prop => prop.Name == propertyName && prop.Value.Equals(propertyValue)))
                .ToList();
        }
    }
}
