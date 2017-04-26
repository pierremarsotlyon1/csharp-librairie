using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R
{
    public class TourJson
    {
        public string Status { get; set; }
        public string Offset { get; set; }
        public Dictionary<string, string>[] Data { get; set; }
    }
}
