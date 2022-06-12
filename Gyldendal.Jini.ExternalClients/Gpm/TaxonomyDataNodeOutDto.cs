namespace Gyldendal.Jini.ExternalClients.Gpm
{
    public class TaxonomyDataNodeOutDto
    {
        public int NodeId { get; set; }
        public string Name { get; set; }
        public System.Collections.Generic.ICollection<int> ChildrenIds { get; set; }
        public int? ParentNodeId { get; set; }
        public int Level { get; set; }
    }
}
