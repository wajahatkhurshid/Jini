using Gyldendal.Jini.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gyldendal.Jini.Services.Common
{
    public static class ProcessGradeLevels
    {
        public static string SortGradeLevels(List<GradeLevel> gradeLevelList)
        {
            var numRegex = new Regex(@"^\d+");
            var gradeLevels = string.Join(",", gradeLevelList.Select(x => x.SortLevel).Distinct()
                .OrderBy(x => Convert.ToInt32(numRegex.Match(x).Value))
                .ThenBy(x => numRegex.Replace(x, "")));

            return gradeLevels;
        }
    }
}