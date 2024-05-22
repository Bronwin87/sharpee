using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public static class CONSTANTS
    {
        // These are the edge type constants used to build bidirectional nodes in the data store
        public const string EdgeType_IsWithin = "IsWithin";
        public const string EdgeType_Contains = "Contains";
        public const string EdgeType_IsCarriedBy = "IsCarriedBy";
        public const string EdgeType_Holds = "Holds";
        public const string EdgeType_IsIn = "IsIn";
        public const string EdgeType_Hosts = "Hosts";
        public const string EdgeType_IsSupporting = "IsSupporting";
        public const string EdgeType_IsOn = "IsOn";
        public const string EdgeType_LeadsTo = "LeadsTo";

        public const string EdgeType_Direction = "Direction";

        public const string EdgeValue_Direction = "Direction";

    }
}
