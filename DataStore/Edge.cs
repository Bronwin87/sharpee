namespace DataStore
{
    public class Edge
    {
        public string Id1 { get; private set; }
        public string Id2 { get; private set; }
        public string EdgeType { get; private set; }
        public List<Property> Properties { get; private set; }

        public Edge(string id1, string id2, string edgeType, List<Property>? properties)
        {
            Id1 = id1;
            Id2 = id2;
            EdgeType = edgeType;

            if (properties != null)
            {
                this.Properties = properties.ToList<Property>();
            }
            else
            {
                Properties = new List<Property>();
            }
        }
    }

}
