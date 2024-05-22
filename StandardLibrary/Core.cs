using DataStore;
using GrammarLibrary;

namespace StandardLibrary
{
    public class Core
    {
        private World _world;
        private Grammar _grammar;

        public Player Player { get; private set; }

        public Core(World world)
        {
            _grammar = new Grammar();
            _world = world;

            // add standard edge types to world model
            _world.AddEdgeType(CONSTANTS.EdgeType_IsWithin, CONSTANTS.EdgeType_Contains);
            _world.AddEdgeType(CONSTANTS.EdgeType_IsCarriedBy, CONSTANTS.EdgeType_Holds);
            _world.AddEdgeType(CONSTANTS.EdgeType_IsIn, CONSTANTS.EdgeType_Hosts);
            _world.AddEdgeType(CONSTANTS.EdgeType_IsSupporting, CONSTANTS.EdgeType_IsOn);
            _world.AddEdgeType(CONSTANTS.EdgeType_LeadsTo, CONSTANTS.EdgeType_LeadsTo);

            // add the default player character to the world model
            Player = new Player("player", "Player", "You are a scruffy adventurer.");
            _world.AddNode(Player.Id, Player);

            // define standard actions in the grammar service
            var standardTake = _grammar.Verb("take", Take).Noun.End;
            var metaExamine = _grammar.Verb("examine", Examine).Noun.End;
            var standardGoInNoun = _grammar.Verb("go", Go).In.Noun.End;
            var standardGoNoun = _grammar.Verb("go", Go).On.Noun.End;

            // define meta actions
            _grammar.MetaVerb("score", Score);
            _grammar.MetaVerb("restore", Restore);
            _grammar.MetaVerb("restart", Restart);

            // Override the "go" action
            var goInNoun = new List<Token>
            {
                new Token(TokenType.Verb, "go", Go, ActionType.Standard),
                new Token(TokenType.Preposition, "in"),
                new Token(TokenType.Noun)
            };

            _grammar.OverrideActionDelegate("go", goInNoun, CustomGo);
            _world = world;
        }

        public Core CreateLocation(string id, string name, string description)
        {
            Location location = new Location(id, name, description);
            _world.AddNode(location.Id, location);

            // If there is only one location (the one just created), connect the Player to the location with IsIn and Hosts edge types
            if (_world.Nodes.Values.Count(node => node.Data is Location) == 1)
            {
                _world.ConnectNodes(Player.Id, id, CONSTANTS.EdgeType_IsIn, null)  ;
                _world.ConnectNodes(id, Player.Id, CONSTANTS.EdgeType_Hosts, null);
            }

            return this;
        }

        private void ConnectLocations(string id1, string id2, Direction direction)
        {
            string edgeType = CONSTANTS.EdgeType_LeadsTo;
            Edge edge = new Edge(id1, id2, edgeType);
            edge.Properties.Add(new Property(CONSTANTS.EdgeValue_Direction, direction.ToString()));
            _world.Nodes[id1].Edges.Add(edge);
        }

        public Direction GetOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.Northeast:
                    return Direction.Southwest;
                case Direction.East:
                    return Direction.West;
                case Direction.Southeast:
                    return Direction.Northwest;
                case Direction.South:
                    return Direction.North;
                case Direction.Southwest:
                    return Direction.Northeast;
                case Direction.West:
                    return Direction.East;
                case Direction.Northwest:
                    return Direction.Southeast;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.In:
                    return Direction.Out;
                case Direction.Out:
                    return Direction.In;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), "Invalid direction provided.");
            }
        }

        private void Take()
        {
        }

        private void Examine()
        {
        }

        private void Go()
        {
        }

        private void CustomGo()
        {
        }

        private void Score()
        {
        }

        private void Restore()
        {
        }

        private void Restart()
        {
        }
    }
}
