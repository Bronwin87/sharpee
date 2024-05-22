using DataStore;

namespace StoryRunner.DataStore.Tests
{
    [TestClass]
    public class WorldTests
    {
        [TestMethod]
        public void AddNode_Should_AddNodeToGraph()
        {
            // Arrange
            var world = new World();

            // Act
            world.AddNode("node1", "data1");

            // Assert
            Assert.IsTrue(world.Nodes.ContainsKey("node1"));
            Assert.AreEqual("data1", world.Nodes["node1"].Data);
        }

        [TestMethod]
        public void RemoveNode_Should_RemoveNodeFromGraph()
        {
            // Arrange
            var world = new World();
            world.AddNode("node1", "data1");

            // Act
            world.RemoveNode("node1");

            // Assert
            Assert.IsFalse(world.Nodes.ContainsKey("node1"));
        }

        [TestMethod]
        public void ConnectNodes_Should_AddBidirectionalEdgeBetweenNodes()
        {
            // Arrange
            var world = new World();
            world.AddNode("node1", "data1");
            world.AddNode("node2", "data2");

            // Act
            world.ConnectNodes("node1", "node2", "there", "andback");

            // Assert
            var node1Edges = world.Nodes["node1"].Edges;
            Assert.IsTrue(node1Edges.Exists(e => e.EdgeType == "there"));
            Assert.IsTrue(node1Edges.Exists(e => e.Id1 == "node1" && e.Id2 == "node2"));
            var node2Edges = world.Nodes["node2"].Edges;
            Assert.IsTrue(node2Edges.Exists(e => e.EdgeType == "andback"));
            Assert.IsTrue(node2Edges.Exists(e => e.Id1 == "node2" && e.Id2 == "node1"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ConnectNodes_Should_ThrowException_WhenReverseEdgeTypeIsNull()
        {
            // Arrange
            var world = new World();
            world.AddNode("node1", "data1");
            world.AddNode("node2", "data2");

            // Act
            world.ConnectNodes("node1", "node2", "edge1", null);
        }

        [TestMethod]
        public void DisconnectNodes_Should_RemoveEdgeBetweenNodes()
        {
            // Arrange
            var world = new World();
            world.AddNode("node1", "data1");
            world.AddNode("node2", "data2");
            world.ConnectNodes("node1", "node2", "edge1", "edge2");

            // Act
            world.DisconnectNodes("node1", "node2");

            // Assert
            Assert.IsFalse(world.Nodes["node1"].Edges.Exists(e => e.Id2 == "node2"));
            Assert.IsFalse(world.Nodes["node2"].Edges.Exists(e => e.Id1 == "node1"));
        }

        [TestMethod]
        public void SetNodeProperty_Should_AddOrUpdateNodeProperty()
        {
            // Arrange
            var world = new World();
            world.AddNode("node1", "data1");

            // Act
            world.SetNodeProperty("node1", "prop1", "value1");
            world.SetNodeProperty("node1", "prop1", "value2");

            // Assert
            Assert.IsTrue(world.Nodes["node1"].Properties.Exists(p => p.Name == "prop1" && p.Value == "value2"));
        }

        [TestMethod]
        public void SetEdgeProperty_Should_AddOrUpdateEdgeProperty()
        {
            // Arrange
            var world = new World();
            world.AddNode("node1", "data1");
            world.AddNode("node2", "data2");
            world.ConnectNodes("node1", "node2", "edge1", "edge2");

            // Act
            world.SetEdgeProperty("node1", "node2", "prop1", "value1");
            world.SetEdgeProperty("node1", "node2", "prop1", "value2");

            // Assert
            var edge = world.Nodes["node1"].Edges.Find(e => e.Id2 == "node2");
            Assert.IsTrue(edge.Properties.Exists(p => p.Name == "prop1" && p.Value == "value2"));
        }
    }
}
