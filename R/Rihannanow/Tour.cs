using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R
{
    [Serializable]
    public class Tour
    {
        public string Title { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Lieu { get; set; }
    }
}
