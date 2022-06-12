namespace Gyldendal.Jini.ExternalClients.Gpm
{
    public class TaxonomyDataOutDto
    {
        public int TaxonomyId { get; set; }

        public System.Collections.Generic.ICollection<int> RootNodeIds { get; set; }

        public System.Collections.Generic.ICollection<string> LevelHeads { get; set; }

        public System.Collections.Generic.ICollection<TaxonomyDataNodeOutDto> Nodes { get; set; }


    }
}
