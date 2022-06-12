using Gyldendal.Jini.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Gyldendal.Jini.Services.Common.Services.RapServiceContract
{
    public class DigitalProductResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UnderTitle { get; set; }

        // ReSharper disable once InconsistentNaming
        public string ISBN13 { get; set; }

        // ReSharper disable once InconsistentNaming
        public string ISBN10 { get; set; }

        public MediaType MediaType { get; set; }
        public MaterialType MaterialType { get; set; }
        public Department Department { get; set; }
        public DigitalProductSection Section { get; set; }
        public List<DigitalProductAuthor> Authors { get; set; }

        public DateTime? ReleaseDate { get; set; }
    }
}