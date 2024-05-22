using DataStore;

namespace StoryRunner.DataStore.Tests
{
    [TestClass]
    public class DataStoreGraphStateChangeTests
    {
        private class MockGraphEventHandler : IGraphEventHandler
        {
            public List<GraphEvent> ReceivedEvents { get; } = new List<GraphEvent>();

            public void HandleNodeAdded(Node node)
            {
                ReceivedEvents.Add(new NodeAddedEvent(node));
            }

            public void HandleNodeRemoved(Node node)
            {
                ReceivedEvents.Add(new NodeRemovedEvent(node));
            }

            public void HandleEdgeAdded(Edge edge)
            {
                ReceivedEvents.Add(new EdgeAddedEvent(edge));
            }

            public void HandleEdgeRemoved(Edge edge)
            {
                ReceivedEvents.Add(new EdgeRemovedEvent(edge));
            }

            public void HandlePropertyChanged(string nodeOrEdgeId, Property property)
            {
                ReceivedEvents.Add(new PropertyChangedEvent(nodeOrEdgeId, property));
            }
        }

        [TestMethod]
        public void AddNode_Should_PublishNodeAddedEvent()
        {
            // Arrange
            var world = new World();
            var eventHandler = new MockGraphEventHandler();
            world.AddEventHandler(eventHandler);

            // Act
            world.AddNode(new Property("name", "name1"), new Property("type", "type1"));

            // Assert
            Assert.AreEqual(1, eventHandler.ReceivedEvents.Count);
            Assert.IsInstanceOfType(eventHandler.ReceivedEvents[0], typeof(NodeAddedEvent));
            Assert.AreEqual("node1", ((NodeAddedEvent)eventHandler.ReceivedEvents[0]).AddedNode.Id);
        }

        [TestMethod]
        public void RemoveNode_Should_PublishNodeRemovedEvent()
        {
            // Arrange
            var world = new World();
            var eventHandler = new MockGraphEventHandler();
            world.AddEventHandler(eventHandler);
            world.AddNode("node1", "data1");

            // Act
            world.RemoveNode("node1");

            // Assert
            Assert.AreEqual(2, eventHandler.ReceivedEvents.Count);
            Assert.IsInstanceOfType(eventHandler.ReceivedEvents[1], typeof(NodeRemovedEvent));
            Assert.AreEqual("node1", ((NodeRemovedEvent)eventHandler.ReceivedEvents[1]).RemovedNode.Id);
        }

        [TestMethod]
        public void ConnectNodes_Should_PublishEdgeAddedEvents()
        {
            // Arrange
            var world = new World();
            var eventHandler = new MockGraphEventHandler();
            world.AddEventHandler(eventHandler);
            world.AddNode("node1", "data1");
            world.AddNode("node2", "data2");

            // Act
            world.ConnectNodes("node1", "node2", "edge1", "edge2");

            // Assert
            Assert.AreEqual(4, eventHandler.ReceivedEvents.Count);
            Assert.IsInstanceOfType(eventHandler.ReceivedEvents[2], typeof(EdgeAddedEvent));
            Assert.IsInstanceOfType(eventHandler.ReceivedEvents[3], typeof(EdgeAddedEvent));
            Assert.AreEqual("node1", ((EdgeAddedEvent)eventHandler.ReceivedEvents[2]).AddedEdge.Id1);
            Assert.AreEqual("node2", ((EdgeAddedEvent)eventHandler.ReceivedEvents[3]).AddedEdge.Id1);
        }

        [TestMethod]
        public void DisconnectNodes_Should_PublishEdgeRemovedEvents()
        {
            // Arrange
            var world = new World();
            var eventHandler = new MockGraphEventHandler();
            world.AddEventHandler(eventHandler);
            world.AddNode("node1", "data1");
            world.AddNode("node2", "data2");
            world.ConnectNodes("node1", "node2", "edge1", "edge2");

            // Act
            world.DisconnectNodes("node1", "node2");

            // Assert
            Assert.AreEqual(6, eventHandler.ReceivedEvents.Count);
            Assert.IsInstanceOfType(eventHandler.ReceivedEvents[4], typeof(EdgeRemovedEvent));
            Assert.IsInstanceOfType(eventHandler.ReceivedEvents[5], typeof(EdgeRemovedEvent));
            Assert.AreEqual("node1", ((EdgeRemovedEvent)eventHandler.ReceivedEvents[4]).RemovedEdge.Id1);
            Assert.AreEqual("node2", ((EdgeRemovedEvent)eventHandler.ReceivedEvents[5]).RemovedEdge.Id1);
        }

        [TestMethod]
        public void SetNodeProperty_Should_PublishPropertyChangedEvent()
        {
            // Arrange
            var world = new World();
            var eventHandler = new MockGraphEventHandler();
            world.AddEventHandler(eventHandler);
            world.AddNode("node1", "data1");

            // Act
            world.SetNodeProperty("node1", "prop1", "value1");

            // Assert
            Assert.AreEqual(2, eventHandler.ReceivedEvents.Count);
            Assert.IsInstanceOfType(eventHandler.ReceivedEvents[1], typeof(PropertyChangedEvent));
            Assert.AreEqual("node1", ((PropertyChangedEvent)eventHandler.ReceivedEvents[1]).NodeOrEdgeId);
            Assert.AreEqual("prop1", ((PropertyChangedEvent)eventHandler.ReceivedEvents[1]).Property.Name);
            Assert.AreEqual("value1", ((PropertyChangedEvent)eventHandler.ReceivedEvents[1]).Property.Value);
        }

        [TestMethod]
        public void SetEdgeProperty_Should_PublishPropertyChangedEvent()
        {
            // Arrange
            var world = new World();
            var eventHandler = new MockGraphEventHandler();
            world.AddEventHandler(eventHandler);
            world.AddNode("node1", "data1");
            world.AddNode("node2", "data2");
            world.ConnectNodes("node1", "node2", "edge1", "edge2");

            // Act
            world.SetEdgeProperty("node1", "node2", "prop1", "value1");

            // Assert
            Assert.AreEqual(5, eventHandler.ReceivedEvents.Count);
            Assert.IsInstanceOfType(eventHandler.ReceivedEvents[4], typeof(PropertyChangedEvent));
            Assert.AreEqual("node1-node2", ((PropertyChangedEvent)eventHandler.ReceivedEvents[4]).NodeOrEdgeId);
            Assert.AreEqual("prop1", ((PropertyChangedEvent)eventHandler.ReceivedEvents[4]).Property.Name);
            Assert.AreEqual("value1", ((PropertyChangedEvent)eventHandler.ReceivedEvents[4]).Property.Value);
        }
    }
}
