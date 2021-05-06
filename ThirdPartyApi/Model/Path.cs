using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdPartyApi.Model
{
    public class Path
    {
        public Path()
        {
            From = new List<Path>();
        }
        public string Name { get; set; }
        public IList<Path> From { get; set; }
    }
}
