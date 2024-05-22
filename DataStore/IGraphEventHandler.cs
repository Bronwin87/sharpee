using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public interface IGraphEventHandler
    {
        void HandleNodeAdded(Node node);
        void HandleNodeRemoved(Node node);
        void HandleEdgeAdded(Edge edge);
        void HandleEdgeRemoved(Edge edge);
        void HandlePropertyChanged(string nodeOrEdgeId, Property property);
    }
}
