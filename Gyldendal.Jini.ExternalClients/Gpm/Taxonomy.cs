using System.Collections.Generic;

namespace Gyldendal.Jini.ExternalClients.Gpm
{
    public class Taxonomy
    {
        public int Id { get; set; }
        public List<int> RootNodeIds { get; set; }
        public List<TaxonomyNode> TaxonomyNodes { get; set; }
    }
}
