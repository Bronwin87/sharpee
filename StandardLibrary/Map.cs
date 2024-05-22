using DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public class Map
    {
        private World world;

        public Map(World world)
        {
            this.world = world;
        }

        public Map DefineRoom(string roomId, string description)
        {
            world.AddNode(roomId, description);
            return this;
        }

        public Map DefineRoomWithName(string roomId, string name, string description, params string[] synonyms)
        {
            DefineRoom(roomId, description);
            SetNodeName(roomId, name, synonyms);
            return this;
        }

        public Map DefineDirectPath(string fromRoomId, string toRoomId, string direction)
        {
            string reverseDirection = GetReverseDirection(direction);
            world.AddEdgeType(direction, reverseDirection);
            world.ConnectNodes(fromRoomId, toRoomId, direction, reverseDirection);
            return this;
        }

        public Map DefineDoor(string doorId, string fromRoomId, string toRoomId, bool isOpen = true, bool isLocked = false)
        {
            world.AddNode(doorId, null);
            world.AddEdgeType("DOOR", "DOOR");
            world.ConnectNodes(fromRoomId, doorId, "DOOR", "DOOR");
            world.ConnectNodes(doorId, toRoomId, "DOOR", "DOOR");
            world.SetNodeProperty(doorId, "isOpen", isOpen);
            world.SetNodeProperty(doorId, "isLocked", isLocked);
            return this;
        }

        public Map DefineEventBasedEntrance(string roomId, string entranceId, Func<bool> condition)
        {
            world.AddNode(entranceId, null);
            world.AddEdgeType("ENTRANCE", "ENTRANCE");
            world.SetNodeProperty(entranceId, "condition", condition);
            world.ConnectNodes(roomId, entranceId, "ENTRANCE", "ENTRANCE");
            return this;
        }

        public bool RoomExists(string roomId)
        {
            return world.Nodes.ContainsKey(roomId);
        }

        private string GetReverseDirection(string direction)
        {
            return direction switch
            {
                "NORTH" => "SOUTH",
                "SOUTH" => "NORTH",
                "EAST" => "WEST",
                "WEST" => "EAST",
                "UP" => "DOWN",
                "DOWN" => "UP",
                "IN" => "OUT",
                "OUT" => "IN",
                _ => throw new ArgumentException($"Invalid direction: {direction}"),
            };
        }

        private void SetNodeName(string nodeId, string name, params string[] synonyms)
        {
            world.SetNodeProperty(nodeId, "name", name);
            world.SetNodeProperty(nodeId, "synonyms", synonyms);
        }
    }
}
