using System.Collections.Generic;

namespace Gyldendal.Jini.ExternalClients.Gpm
{
    public class TaxonomyNode
    {
        public int NodeId { get; set; }
        public string Name { get; set; }
        public List<int> ChildNodeIds { get; set; }
        public int? ParentNodeId { get; set; }
        public int Level { get; set; }
    }
}
