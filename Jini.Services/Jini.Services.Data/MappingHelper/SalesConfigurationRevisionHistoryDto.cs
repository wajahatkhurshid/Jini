
using Gyldendal.Jini.Services.Data;
using Gyldendal.Jini.Services.Data.MappingHelper.RevsionHistory;
using System;
using System.Collections.Generic;

namespace Gyldendal.Jini.Services.Common.Dtos
{
   public class SalesConfigurationRevisionHistoryDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public SalesConfiguration SalesConfiguration { get; set; }
        public string CreatedBy { get; set; }
        public int VersionNo { get; set; }
        public string Isbn { get; set; }
        public List<RevisionHistoryData> RevisionHistoryData { get; set; }
        
    }
}
