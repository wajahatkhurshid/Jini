using System;
using System.Collections.Generic;

namespace Gyldendal.Jini.Services.Contracts
{
    public class ProductContainer //: ContainerBase, ProductUpserted
    {
        public int ContainerInstanceId { get; set; }
        public string ProductISBN13 { get; set; }

        public string ProductTitle { get; set; }

        public string ProductSubtitle { get; set; }


        public List<List<GpmNode>> ProductMediaMaterialeType { get; set; }


        public string RelatedProductFirstEditionProductPublishedDate { get; set; }

        public DateTime? ProductPublishedDate { get; set; }

        public List<List<GpmNode>> ProductEducationSubjectLevel { get; set; }
        public int? ProductEdition { get; set; }
        public bool IsNextPrintRunPlanned { get; set; }
        public List<List<GpmNode>> ProductEditorialDivision { get; set; }
    }
}
